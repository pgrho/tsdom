using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    [TestClass]
    public class VariableDeclarationStatementTest
    {
        private class TestClass
        {
            public int SourceMethod()
            {
                var a = 1;
                const int b = 2;

                return a + b;
            }
        }

        [TestMethod]
        public void VariableDeclarationStatement_TransformTest()
        {
            var t = new TransformingContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}