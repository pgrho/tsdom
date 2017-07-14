using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    [TestClass]
    public class ReturnStatementTest
    {
        public class TestClass
        {
            public int SourceMethod()
                => 1;
        }

        [TestMethod]
        public void ReturnStatement_TransformTest()
        {
            var t = new TransformingContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}