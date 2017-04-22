using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    public class DoStatementTestClass
    {
        public void SourceMethod(int a)
        {
            do
            {
                ToString();
            }
            while (a-- > 0);
        }
    }

    [TestClass]
    public class DoStatementTest : TransformationTestBase<DoStatementTestClass>
    {
        [TestMethod]
        public void TransformTest()
        {
            var m = GetMethod(nameof(DoStatementTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}
