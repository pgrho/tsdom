using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    public class TryStatementTestClass
    {
        public string SourceMethod(object obj)
        {
            try
            {
                return obj.ToString();
            }
            catch
            {
                return null;
            }
        }
    }

    [TestClass]
    public class TryStatementTest : TransformationTestBase<TryStatementTestClass>
    {
        [TestMethod]
        public void TryStatement_TransformTest()
        {
            var m = GetMethod(nameof(TryStatementTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}
