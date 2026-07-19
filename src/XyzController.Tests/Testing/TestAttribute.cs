using System;

namespace XyzController.Tests.Testing
{
    /// <summary>
    /// 标记一个类为测试夹具（fixture）。TestRunner 会扫描带此特性的类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TestFixtureAttribute : Attribute
    {
    }

    /// <summary>
    /// 标记一个方法为测试用例。方法必须无参、返回 void。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class TestAttribute : Attribute
    {
        public string Description { get; set; }

        public TestAttribute() { }

        public TestAttribute(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// 标记一个方法为初始化方法，在每个测试用例之前调用。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class SetupAttribute : Attribute
    {
    }
}
