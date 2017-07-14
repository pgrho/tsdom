using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    [TestClass]
    public class WhileStatementTest
    {
        private class TestClass
        {
            public void SourceMethod(int a)
            {
                while (a-- > 0)
                {
                    ToString();
                }
            }
        }

        [TestMethod]
        public void WhileStatement_TransformTest()
        {
            var t = new TransformingContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}