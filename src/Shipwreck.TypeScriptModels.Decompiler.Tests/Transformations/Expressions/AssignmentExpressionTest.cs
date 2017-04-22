using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Expressions
{
    public class AssignmentExpressionTestClass
    {
        public int SourceMethod(int a)
        {
            a = a + a;
            return a -= 5;
        }
    }

    [TestClass]
    public class AssignmentExpressionTest : TransformationTestBase<AssignmentExpressionTestClass>
    {
        [TestMethod]
        public void TransformTest()
        {
            var m = GetMethod(nameof(AssignmentExpressionTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}