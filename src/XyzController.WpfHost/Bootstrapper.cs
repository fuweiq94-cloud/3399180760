using System;
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
    /// - WpfHost 项目不直接引用 XyzController 项目（避免循环依赖）；
    /// - EXE 在运行时通过反射从同目录的 XyzController.dll 加载 MainForm；
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
            string dllPath = Path.Combine(exeDir, "XyzController.dll");

            if (!File.Exists(dllPath))
            {
                MessageBox.Show(
                    "未找到 XyzController.dll，请将其放置于 EXE 同目录下。\n\n路径：" + dllPath,
                    "XyzController.WpfHost",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return 1;
            }

            // 通过反射加载 XyzController.dll 并创建 MainForm 实例
            Assembly asm = Assembly.LoadFrom(dllPath);
            Type formType = asm.GetType("XyzController.MainForm");
            if (formType == null)
            {
                MessageBox.Show(
                    "XyzController.dll 中未找到 XyzController.MainForm 类型。",
                    "XyzController.WpfHost",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return 1;
            }

            Form form = (Form)Activator.CreateInstance(formType);
            return WpfHostLauncher.Run(form);
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
