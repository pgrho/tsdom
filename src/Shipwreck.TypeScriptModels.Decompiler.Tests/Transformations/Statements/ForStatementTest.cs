using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    [TestClass]
    public class ForStatementTest
    {
        private class TestClass
        {
            public void SourceMethod(int[] a)
            {
                for (var n = 0; n < a.Length; n++)
                {
                    a[n].ToString();
                }
            }
        }

        [TestMethod]
        public void ForStatement_TransformTest()
        {
            var t = new TransformingContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}