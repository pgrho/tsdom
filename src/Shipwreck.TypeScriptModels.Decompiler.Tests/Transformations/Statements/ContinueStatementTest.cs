using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    public class ContinueStatementTestClass
    {
        public void SourceMethod(int[] a)
        {
            foreach (var e in a)
            {
                if (e == 0)
                {
                    continue;
                }
                e.ToString();
            }
        }
    }

    [TestClass]
    public class ContinueStatementTest : TransformationTestBase<ContinueStatementTestClass>
    {
        [TestMethod]
        public void ContinueStatement_TransformTest()
        {
            var m = GetMethod(nameof(ContinueStatementTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}
