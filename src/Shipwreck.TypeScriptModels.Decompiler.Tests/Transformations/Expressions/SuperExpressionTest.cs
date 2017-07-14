using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Expressions
{
    [TestClass]
    public class SuperExpressionTest
    {
        private class TestClass
        {
            public string SourceMethod(int a)
                => base.ToString();
        }

        [TestMethod]
        public void SuperExpression_TransformTest()
        {
            var t = new TransformingContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}