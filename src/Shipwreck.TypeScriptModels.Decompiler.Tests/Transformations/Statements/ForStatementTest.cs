using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    public class ForStatementTestClass
    {
        public void SourceMethod(int[] a)
        {
            for (var n = 0; n < a.Length; n++)
            {
                a[n].ToString();
            }
        }
    }

    [TestClass]
    public class ForStatementTest : TransformationTestBase<ForStatementTestClass>
    {
        [TestMethod]
        public void TransformTest()
        {
            var m = GetMethod(nameof(ForStatementTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}
