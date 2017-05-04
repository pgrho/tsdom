using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    public class ForOfStatementTestClass
    {
        public void SourceMethod(IEnumerable a)
        {
            foreach (var e in a)
            {
                e.ToString();
            }
        }
    }

    [TestClass]
    public class ForOfStatementTest : TransformationTestBase<ForOfStatementTestClass>
    {
        [TestMethod]
        public void ForOfStatement_TransformTest()
        {
            var m = GetMethod(nameof(ForOfStatementTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}
