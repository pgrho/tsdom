using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    [TestClass]
    public class ContinueStatementTest
    {
        private class TestClass
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

        [TestMethod]
        public void ContinueStatement_Test()
        {
            var t = new TypeTranslationContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}