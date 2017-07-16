using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    [TestClass]
    public class ExpressionStatementTest
    {
        private class TestClass
        {
            public void SourceMethod()
            {
                ToString();
            }
        }

        [TestMethod]
        public void ExpressionStatement_Test()
        {
            var t = new TypeTranslationContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}