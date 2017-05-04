using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    public class ReturnStatementTestClass
    {
        public int SourceMethod()
            => 1;
    }

    [TestClass]
    public class ReturnStatementTest : TransformationTestBase<ReturnStatementTestClass>
    {
        [TestMethod]
        public void ReturnStatement_TransformTest()
        {
            var m = GetMethod(nameof(ReturnStatementTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}
