using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace XyzController.WpfHost
{
    /// <summary>
    /// 通用启动器：扫描 EXE 同目录下的业务 DLL，找到第一个含 Program.Main(string[])
    /// 静态方法的类型，反射调用它（透传命令行参数）。
    ///
    /// 设计目标：
    /// - WpfHost 项目（含本文件）是"框架"，永远不需要再修改。
    /// - 业务方（如 XyzController）只需在自己的 Program.cs 里写：
    ///       WpfHostLauncher.Run(pages);
    ///   即可决定注册哪些 Form 到导航栏。
    /// - 把 XyzController.WpfHost.dll + XyzController.WpfHost.Launcher.exe 复制到任何业务
    ///   项目的输出目录，都能用，业务方完全不接触 WpfHost 框架源码。
    ///
    /// 设计要点：
    /// - EXE 文件名为 XyzController.WpfHost.Launcher.exe（与 DLL 的 XyzController.WpfHost
    ///   文件名不同），所以 AssemblyName 不同。这样 CLR 不会把 EXE 当作 WpfHost DLL，
    ///   业务代码引用 WpfHostLauncher 才能正确从 DLL 加载。
    /// - 业务 Program.Main 用 private static，反射时需 BindingFlags.NonPublic。
    /// - 命令行参数原样透传，业务方可以自己解析（如 XyzController 的 main/jump/nav 模式）。
    /// </summary>
    internal static class Bootstrapper
    {
        [STAThread]
        private static int Main(string[] args)
        {
            // 程序集解析回退：当默认探测找不到程序集时，尝试从 EXE 所在目录加载
            // （业务程序集加载 XyzController.Controls.dll 等间接依赖时会触发）
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string logPath = Path.Combine(exeDir, "bootstrapper.log");

            try
            {
                return FindAndInvokeProgramMain(exeDir, args);
            }
            catch (Exception ex)
            {
                try { File.WriteAllText(logPath, ex.ToString()); } catch { }
                MessageBox.Show(
                    "启动失败：" + ex.Message + "\n\n详细日志已写入：" + logPath,
                    "XyzController.WpfHost",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return 1;
            }
        }

        /// <summary>
        /// 扫描 exeDir 下所有业务 DLL，找到第一个含 Program.Main(string[]) 的类型并调用。
        /// </summary>
        private static int FindAndInvokeProgramMain(string exeDir, string[] args)
        {
            // 自己（启动器）的程序集名不算业务 dll
            string selfAsmName = typeof(Bootstrapper).Assembly.GetName().Name;

            string[] dlls = Directory.GetFiles(exeDir, "*.dll");
            string scannedList = string.Empty;

            foreach (string dllPath in dlls)
            {
                string fileName = Path.GetFileName(dllPath);

                // 1) 跳过自身（启动器 EXE 名字是 XyzController.WpfHost.Launcher.dll，不可能在这里出现，但保险起见）
                if (fileName.StartsWith(selfAsmName + ".", StringComparison.OrdinalIgnoreCase))
                    continue;

                // 2) 跳过已知框架 DLL（避免反射触发大量框架初始化）
                if (IsFrameworkDll(fileName))
                    continue;

                Assembly asm;
                try
                {
                    asm = Assembly.LoadFrom(dllPath);
                }
                catch (BadImageFormatException)
                {
                    // 非托管 C++ DLL、图标资源 DLL 等 —— 静默跳过
                    continue;
                }
                catch (Exception)
                {
                    // 加载失败也跳过，继续扫描下一个
                    continue;
                }

                // 收集扫描记录，便于出错时调试
                if (scannedList.Length > 0) scannedList += ", ";
                scannedList += fileName;

                // 3) 找含 Main(string[]) 的类型
                MethodInfo mainMethod = FindProgramMain(asm);
                if (mainMethod == null) continue;

                // 4) 找到了 → 反射调用并退出（只调第一个，用户选择）
                return InvokeMain(mainMethod, args);
            }

            // 全部扫完都没找到
            MessageBox.Show(
                "在 EXE 同目录下未找到任何含 Program.Main(string[]) 的业务 DLL。\n\n" +
                "请把业务项目的 dll（如 XyzController.dll）放到本目录：\n  " + exeDir + "\n\n" +
                "已扫描的 DLL：" + (scannedList.Length > 0 ? scannedList : "（无）"),
                "XyzController.WpfHost",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return 1;
        }

        /// <summary>
        /// 在程序集中查找含 private/public static Main(string[]) 的类型。
        /// 通常业务方约定俗成写在 static class Program 里。
        /// </summary>
        private static MethodInfo FindProgramMain(Assembly asm)
        {
            Type[] types;
            try
            {
                types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException)
            {
                // 业务程序集依赖了找不到的类型时，GetTypes 会抛这个异常
                // 此时仍然可以从 LoadedExceptions 里找
                return null;
            }

            foreach (Type t in types)
            {
                if (t == null) continue;
                // 只看类，不看接口/值类型
                if (!t.IsClass) continue;

                MethodInfo main = t.GetMethod(
                    "Main",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    CallingConventions.Standard,
                    new Type[] { typeof(string[]) },
                    null);

                if (main != null) return main;
            }
            return null;
        }

        /// <summary>
        /// 反射调用 Program.Main(string[])，处理返回值（int 或 void）。
        /// </summary>
        private static int InvokeMain(MethodInfo mainMethod, string[] args)
        {
            object retVal = mainMethod.Invoke(null, new object[] { args });

            // Program.Main 可能是 void 也可能是 int
            if (retVal is int)
                return (int)retVal;
            return 0;
        }

        /// <summary>
        /// 判断给定 dll 文件名是否是已知的 .NET 框架/WPF 框架 DLL。
        /// 这些 DLL 不太可能含业务 Program.Main，跳过以避免反射初始化开销。
        /// </summary>
        private static bool IsFrameworkDll(string fileName)
        {
            // 已知框架 DLL 前缀列表（按需扩充）
            string[] knownPrefixes = new string[] {
                "System.", "Microsoft.", "mscorlib",
                "PresentationCore", "PresentationFramework", "WindowsBase",
                "WindowsFormsIntegration", "System.Xaml",
                "UIAutomation", "ReachFramework",
                "System.Windows.",
                "XyzController.WpfHost",   // 自身（再加一道保险）
            };

            foreach (string prefix in knownPrefixes)
            {
                if (fileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 程序集解析回退：从 EXE 所在目录加载缺失的程序集
        /// （如业务程序集依赖的 XyzController.Controls.dll）。
        ///
        /// ★ 关键：本 EXE 的 AssemblyName 与 WpfHost DLL 同名（都叫 XyzController.WpfHost），
        ///   CLR 默认会把 EXE 自身当作同名程序集返回，导致业务代码引用 WpfHostLauncher
        ///   时抛 TypeLoadException（EXE 自己没这个类型）。
        ///   因此当请求名 == 本程序集名时，必须强制返回 DLL 路径，不能让 CLR 返回 EXE。
        /// </summary>
        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string name = new AssemblyName(args.Name).Name;
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(exeDir, name + ".dll");
            if (File.Exists(path))
                return Assembly.LoadFrom(path);
            // 尝试 .exe 后缀（罕见，但有的 .NET dll 叫 .exe）
            string exePath = Path.Combine(exeDir, name + ".exe");
            if (File.Exists(exePath))
                return Assembly.LoadFrom(exePath);
            return null;
        }
    }
}
