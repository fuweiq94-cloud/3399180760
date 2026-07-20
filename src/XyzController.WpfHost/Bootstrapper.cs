using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace XyzController.WpfHost
{
    /// <summary>
    /// XyzController.WpfHost.exe 独立运行入口。
    /// 本文件不参与 WpfHost DLL 的编译，仅由 csproj 中 BuildExe 后置目标
    /// 单独编译为 EXE。
    ///
    /// 设计要点：
    /// - EXE 与 DLL 同名（都叫 XyzController.WpfHost），CLR 会将 EXE 自身
    ///   当作同名程序集，导致无法从 DLL 加载类型。因此 EXE 不引用 WpfHost DLL，
    ///   而是在运行时通过反射加载 DLL 并调用 WpfHostLauncher.Run()。
    /// - WpfHost 项目不直接引用 XyzController 项目（避免循环依赖）；
    /// - EXE 在运行时通过反射从同目录的 XyzController.dll 加载 MainForm。
    /// - 部署时须将 XyzController.dll、XyzController.Controls.dll
    ///   与 XyzController.WpfHost.dll 放在 EXE 同目录下。
    /// </summary>
    internal static class Bootstrapper
    {
        [STAThread]
        private static int Main()
        {
            // 程序集解析回退：当默认探测找不到程序集时，尝试从 EXE 所在目录加载
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string logPath = Path.Combine(exeDir, "bootstrapper.log");

            try
            {
                // 1. 加载 XyzController.dll 并创建多个窗体作为导航页面
                string xcDllPath = Path.Combine(exeDir, "XyzController.dll");
                if (!File.Exists(xcDllPath))
                {
                    MessageBox.Show(
                        "未找到 XyzController.dll，请将其放置于 EXE 同目录下。\n\n路径：" + xcDllPath,
                        "XyzController.WpfHost",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return 1;
                }

                Assembly xcAsm = Assembly.LoadFrom(xcDllPath);

                // 创建多页面列表
                List<object> pages = new List<object>();
                AddPage(pages, xcAsm, "XyzController.MainForm", "主控制器");
                AddPage(pages, xcAsm, "XyzController.PointJumpForm", "点位跳转");
                AddPage(pages, xcAsm, "XyzController.TrajectoryViewForm", "运动轨迹");

                if (pages.Count == 0)
                {
                    MessageBox.Show(
                        "XyzController.dll 中未找到任何可用的窗体类型。",
                        "XyzController.WpfHost",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return 1;
                }

                // 2. ★ 通过反射加载 WpfHost DLL 并调用 WpfHostLauncher.Run(IList<WpfPage>)
                return LaunchViaReflection(exeDir, pages);
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
        /// 从程序集中加载指定窗体类型并创建 WpfPage 对象加入列表。
        /// </summary>
        private static void AddPage(List<object> pages, Assembly asm, string typeName, string title)
        {
            Type formType = asm.GetType(typeName);
            if (formType == null) return;

            Form form = (Form)Activator.CreateInstance(formType);

            // 通过反射创建 WpfPage（避免编译时引用 WpfHost DLL）
            // WpfPage 在 WpfHost DLL 中，但 EXE 不能直接引用它
            // 所以我们直接传 Form 列表和标题列表给 LaunchViaReflection
            pages.Add(new object[] { title, form });
        }

        /// <summary>
        /// 通过反射加载 WpfHost DLL 并调用 WpfHostLauncher.Run(IList&lt;WpfPage&gt;)。
        /// 独立方法确保 JIT 在调用时才解析相关类型。
        /// </summary>
        private static int LaunchViaReflection(string exeDir, List<object> pages)
        {
            string whDllPath = Path.Combine(exeDir, "XyzController.WpfHost.dll");
            if (!File.Exists(whDllPath))
            {
                MessageBox.Show(
                    "未找到 XyzController.WpfHost.dll，请将其放置于 EXE 同目录下。",
                    "XyzController.WpfHost",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return 1;
            }

            Assembly whAsm = Assembly.LoadFrom(whDllPath);

            // 创建 WpfPage 实例列表
            Type pageType = whAsm.GetType("XyzController.WpfHost.WpfPage", true);
            Type listType = typeof(List<>).MakeGenericType(pageType);
            object pageList = Activator.CreateInstance(listType);
            MethodInfo addMethod = listType.GetMethod("Add");

            foreach (object item in pages)
            {
                object[] pair = (object[])item;
                string title = (string)pair[0];
                Form form = (Form)pair[1];
                object page = Activator.CreateInstance(pageType, title, form);
                addMethod.Invoke(pageList, new object[] { page });
            }

            // 调用 WpfHostLauncher.Run(IList<WpfPage>)
            Type launcherType = whAsm.GetType("XyzController.WpfHost.WpfHostLauncher", true);
            Type ilistType = typeof(IList<>).MakeGenericType(pageType);
            MethodInfo runMethod = launcherType.GetMethod("Run", new[] { ilistType });
            return (int)runMethod.Invoke(null, new object[] { pageList });
        }

        /// <summary>
        /// 程序集解析回退：从 EXE 所在目录加载缺失的程序集
        /// （如 XyzController.Controls.dll 等间接依赖）。
        /// </summary>
        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string name = new AssemblyName(args.Name).Name;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name + ".dll");
            return File.Exists(path) ? Assembly.LoadFrom(path) : null;
        }
    }
}
