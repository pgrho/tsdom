using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    public class VariableDeclarationStatementTestClass
    {
        public int SourceMethod()
        {
            var a = 1;
            const int b = 2;

            return a + b;
        }
    }

    [TestClass]
    public class VariableDeclarationStatementTest : TransformationTestBase<VariableDeclarationStatementTestClass>
    {
        [TestMethod]
        public void VariableDeclarationStatement_TransformTest()
        {
            var m = GetMethod(nameof(VariableDeclarationStatementTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}
