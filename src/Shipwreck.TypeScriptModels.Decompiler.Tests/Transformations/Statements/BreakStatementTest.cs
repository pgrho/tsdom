using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    public class BreakStatementTestClass
    {
        public void SourceMethod(int[] a)
        {
            foreach (var e in a)
            {
                if (e == 0)
                {
                    break;
                }
            }
        }
    }

    [TestClass]
    public class BreakStatementTest : TransformationTestBase<BreakStatementTestClass>
    {
        [TestMethod]
        public void BreakStatement_TransformTest()
        {
            var m = GetMethod(nameof(BreakStatementTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}
