using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Expressions
{
    public class UnaryExpressionTestClass
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

    [TestClass]
    public class UnaryExpressionTest : TransformationTestBase<UnaryExpressionTestClass>
    {
        [TestMethod]
        public void UnaryExpression_TransformTest()
        {
            var m = GetMethod(nameof(UnaryExpressionTestClass.UnaryPlus));

            Assert.IsNotNull(m);
        }
    }
}
