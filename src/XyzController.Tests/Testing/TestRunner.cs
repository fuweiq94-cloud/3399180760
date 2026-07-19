using System;
using System.Collections.Generic;
using System.Reflection;

namespace XyzController.Tests.Testing
{
    /// <summary>
    /// 轻量级测试运行器。
    /// 通过反射扫描带 [TestFixture] 的类，找到带 [Test] 的方法，依次执行。
    /// 支持 [Setup] 方法（每个测试前调用）。
    /// 输出格式仿照 NUnit：每个测试一行结果（PASS/FAIL）+ 末尾汇总。
    /// </summary>
    public static class TestRunner
    {
        // ANSI 颜色（cmd 不一定支持，但 Windows Terminal / VS 输出窗口支持）
        private const string ColorReset = "\x1b[0m";
        private const string ColorGreen = "\x1b[32m";
        private const string ColorRed = "\x1b[31m";
        private const string ColorYellow = "\x1b[33m";
        private const string ColorCyan = "\x1b[36m";
        private const string ColorDim = "\x1b[2m";

        public static int RunAll(params Assembly[] assemblies)
        {
            int totalPass = 0;
            int totalFail = 0;
            int totalSkip = 0;
            var failures = new List<string>();

            foreach (Assembly asm in assemblies)
            {
                foreach (Type type in asm.GetTypes())
                {
                    if (!type.IsClass) continue;
                    if (type.GetCustomAttribute<TestFixtureAttribute>() == null) continue;

                    Console.WriteLine();
                    Console.WriteLine(ColorCyan + "━━ " + type.FullName + ColorReset);
                    Console.WriteLine(ColorDim + new string('─', 60) + ColorReset);

                    // 创建 fixture 实例
                    object fixture;
                    try
                    {
                        fixture = Activator.CreateInstance(type);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ColorRed + "  [ERROR] 无法创建实例: " + ex.GetBaseException().Message + ColorReset);
                        continue;
                    }

                    // 找 Setup 方法
                    MethodInfo setupMethod = null;
                    foreach (MethodInfo m in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (m.GetCustomAttribute<SetupAttribute>() != null)
                        {
                            setupMethod = m;
                            break;
                        }
                    }

                    // 找所有 Test 方法
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        TestAttribute attr = method.GetCustomAttribute<TestAttribute>();
                        if (attr == null) continue;

                        string testName = type.Name + "." + method.Name;
                        string desc = attr.Description;

                        // 调用 Setup
                        try
                        {
                            if (setupMethod != null) setupMethod.Invoke(fixture, null);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("  " + ColorYellow + "[SKIP]" + ColorReset + " " + testName +
                                              "  Setup 失败: " + ex.GetBaseException().Message);
                            totalSkip++;
                            continue;
                        }

                        // 调用 Test
                        try
                        {
                            method.Invoke(fixture, null);
                            Console.WriteLine("  " + ColorGreen + "[PASS]" + ColorReset + " " + testName +
                                              (string.IsNullOrEmpty(desc) ? "" : ColorDim + "  // " + desc + ColorReset));
                            totalPass++;
                        }
                        catch (Exception ex)
                        {
                            Exception root = ex.GetBaseException();
                            string why = root is AssertionException ? root.Message : (root.GetType().Name + ": " + root.Message);
                            Console.WriteLine("  " + ColorRed + "[FAIL]" + ColorReset + " " + testName +
                                              (string.IsNullOrEmpty(desc) ? "" : ColorDim + "  // " + desc + ColorReset));
                            Console.WriteLine("        " + ColorRed + why + ColorReset);
                            failures.Add(testName + ": " + why);
                            totalFail++;
                        }
                    }
                }
            }

            // 汇总
            Console.WriteLine();
            Console.WriteLine(ColorDim + new string('═', 60) + ColorReset);
            Console.Write("  测试结果: ");
            string summaryColor = totalFail == 0 ? ColorGreen : ColorRed;
            Console.WriteLine(summaryColor + totalPass + " 通过" + ColorReset +
                              ", " + ColorRed + totalFail + " 失败" + ColorReset +
                              ", " + ColorYellow + totalSkip + " 跳过" + ColorReset +
                              "（共 " + (totalPass + totalFail + totalSkip) + " 个）");

            // 失败详情
            if (failures.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine(ColorRed + "  失败列表：" + ColorReset);
                foreach (string f in failures)
                {
                    Console.WriteLine(ColorRed + "    ✗ " + f + ColorReset);
                }
            }

            Console.WriteLine();
            return totalFail;   // 进程退出码：失败数（0 = 全部通过）
        }
    }
}
