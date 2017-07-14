using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    [TestClass]
    public class TryStatementTest
    {
        private class TestClass
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

        [TestMethod]
        public void TryStatement_TransformTest()
        {
            var t = new TransformingContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}