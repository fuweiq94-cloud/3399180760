using System;

namespace XyzController.Tests.Testing
{
    /// <summary>
    /// 轻量级断言库。提供常见的断言方法，失败时抛出 AssertionException。
    /// </summary>
    public static class Assert
    {
        public static void IsTrue(bool condition, string message = null)
        {
            if (!condition)
                throw new AssertionException("期望 true，实际 false。" + (message ?? ""));
        }

        public static void IsFalse(bool condition, string message = null)
        {
            if (condition)
                throw new AssertionException("期望 false，实际 true。" + (message ?? ""));
        }

        public static void AreEqual<T>(T expected, T actual, string message = null)
        {
            if (!object.Equals(expected, actual))
            {
                throw new AssertionException(string.Format(
                    "期望 <{0}>，实际 <{1}>。{2}",
                    Format(expected), Format(actual), message ?? ""));
            }
        }

        public static void AreNotEqual<T>(T notExpected, T actual, string message = null)
        {
            if (object.Equals(notExpected, actual))
            {
                throw new AssertionException(string.Format(
                    "不应该等于 <{0}>，但实际是 <{0}>。{1}",
                    Format(notExpected), message ?? ""));
            }
        }

        /// <summary>浮点数相等（带容差）。</summary>
        public static void AreEqual(float expected, float actual, float tolerance, string message = null)
        {
            if (Math.Abs(expected - actual) > tolerance)
            {
                throw new AssertionException(string.Format(
                    "期望 <{0}>，实际 <{1}>（容差 {2}）。{3}",
                    expected, actual, tolerance, message ?? ""));
            }
        }

        public static void AreEqual(double expected, double actual, double tolerance, string message = null)
        {
            if (Math.Abs(expected - actual) > tolerance)
            {
                throw new AssertionException(string.Format(
                    "期望 <{0}>，实际 <{1}>（容差 {2}）。{3}",
                    expected, actual, tolerance, message ?? ""));
            }
        }

        public static void IsNull(object value, string message = null)
        {
            if (value != null)
                throw new AssertionException("期望 null，实际 <" + Format(value) + ">。" + (message ?? ""));
        }

        public static void IsNotNull(object value, string message = null)
        {
            if (value == null)
                throw new AssertionException("期望非 null，但实际是 null。" + (message ?? ""));
        }

        /// <summary>断言：两个引用指向同一个对象。</summary>
        public static void AreSame(object expected, object actual, string message = null)
        {
            if (!object.ReferenceEquals(expected, actual))
            {
                throw new AssertionException("期望两个引用相同，实际不同。" + (message ?? ""));
            }
        }

        /// <summary>断言：两个引用不指向同一个对象。</summary>
        public static void AreNotSame(object notExpected, object actual, string message = null)
        {
            if (object.ReferenceEquals(notExpected, actual))
            {
                throw new AssertionException("期望两个引用不同，实际相同。" + (message ?? ""));
            }
        }

        /// <summary>断言：执行 code 时会抛出 T 异常。</summary>
        public static T Throws<T>(Action code, string message = null) where T : Exception
        {
            try
            {
                code();
            }
            catch (T ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                throw new AssertionException(string.Format(
                    "期望抛出 <{0}>，但实际抛出 <{1}>。{2}",
                    typeof(T).Name, ex.GetType().Name, message ?? ""));
            }
            throw new AssertionException(string.Format(
                "期望抛出 <{0}>，但没有抛任何异常。{1}",
                typeof(T).Name, message ?? ""));
        }

        private static string Format(object value)
        {
            if (value == null) return "null";
            return value.ToString();
        }
    }

    /// <summary>断言失败异常。TestRunner 捕获后计为"失败"。</summary>
    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message) { }
    }
}
