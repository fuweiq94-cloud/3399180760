using System;
using System.Reflection;
using XyzController.Tests.Testing;

namespace XyzController.Tests
{
    /// <summary>
    /// 测试入口。直接运行此 exe 即可执行所有单元测试。
    /// </summary>
    internal static class Program
    {
        private static int Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("\x1b[1m╔══════════════════════════════════════════════╗\x1b[0m");
            Console.WriteLine("\x1b[1m║   XyzController 业务层单元测试   ║\x1b[0m");
            Console.WriteLine("\x1b[1m╚══════════════════════════════════════════════╝\x1b[0m");

            // verbose 模式（命令行带 -v 显示更多输出）
            if (Array.IndexOf(args, "-v") >= 0 || Array.IndexOf(args, "--verbose") >= 0)
                Console.WriteLine("Verbose 模式已开启");

            Assembly asm = Assembly.GetExecutingAssembly();
            int failCount = TestRunner.RunAll(asm);

            Console.WriteLine("按任意键退出...");
            if (!Console.IsInputRedirected) Console.ReadKey();
            return failCount;
        }
    }
}
