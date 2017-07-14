using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Expressions
{
    [TestClass]
    public class ThisExpressionTest
    {
        private class TestClass
        {
            public TestClass SourceMethod(int a)
                => this;
        }

        [TestMethod]
        public void ThisExpression_TransformTest()
        {
            var t = new TransformingContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}