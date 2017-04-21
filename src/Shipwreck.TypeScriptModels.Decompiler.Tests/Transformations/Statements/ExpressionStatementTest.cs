using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    public class ExpressionStatementTestClass
    {
        public void SourceMethod()
        {
            ToString();
        }
    }

    [TestClass]
    public class ExpressionStatementTest : TransformationTestBase<ExpressionStatementTestClass>
    {
        [TestMethod]
        public void TransformTest()
        {
            var m = GetMethod(nameof(ExpressionStatementTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}
