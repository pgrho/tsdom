using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    [TestClass]
    public class BreakStatementTest
    {
        private class TestClass
        {
            public void SourceMethod(int[] a)
            {
                foreach (var e in a)
                {
                    if (e == 0)
                    {
                        break;
                    }
                }
            }
        }

        [TestMethod]
        public void BreakStatement_TransformTest()
        {
            var t = new TransformingContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}