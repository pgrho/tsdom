using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    public class WhileStatementTestClass
    {
        public void SourceMethod(int a)
        {
            while (a-- > 0)
            {
                ToString();
            }
        }
    }

    [TestClass]
    public class WhileStatementTest : TransformationTestBase<WhileStatementTestClass>
    {
        [TestMethod]
        public void TransformTest()
        {
            var m = GetMethod(nameof(WhileStatementTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}
