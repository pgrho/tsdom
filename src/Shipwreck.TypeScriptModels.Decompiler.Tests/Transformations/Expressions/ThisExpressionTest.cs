using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Expressions
{
    public class ThisExpressionTestClass
    {
        public ThisExpressionTestClass SourceMethod(int a)
            => this;
    }

    [TestClass]
    public class ThisExpressionTest : TransformationTestBase<ThisExpressionTestClass>
    {
        [TestMethod]
        public void ThisExpression_TransformTest()
        {
            var m = GetMethod(nameof(ThisExpressionTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}