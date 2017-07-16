using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Expressions
{
    [TestClass]
    public class UnaryExpressionTest
    {
        private class TestClass
        {
            public int UnaryPlus(int a)
                => +a;

            private int UnaryMinus(int a)
                => -a;

            private int BitwiseNot(int a)
                => ~a;

            private int PrefixIncrement(int a)
                => ++a;

            private int PrefixDecrement(int a)
                => --a;

            private int PostfixIncrement(int a)
                => a++;

            private int PostfixDecrement(int a)
                => a--;
        }

        [TestMethod]
        public void UnaryExpression_Test()
        {
            var t = new TypeTranslationContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.UnaryPlus));

            Assert.IsNotNull(m);
        }
    }
}