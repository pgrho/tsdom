using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Expressions
{
    public class SuperExpressionTestClass
    {
        public string SourceMethod(int a)
            => base.ToString();
    }

    [TestClass]
    public class SuperExpressionTest : TransformationTestBase<SuperExpressionTestClass>
    {
        [TestMethod]
        public void TransformTest()
        {
            var m = GetMethod(nameof(SuperExpressionTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}