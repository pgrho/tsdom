using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    [TestClass]
    public class DoStatementTest
    {
        private class TestClass
        {
            public void SourceMethod(int a)
            {
                do
                {
                    ToString();
                }
                while (a-- > 0);
            }
        }

        [TestMethod]
        public void DoStatement_Test()
        {
            var t = new TypeTranslationContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}