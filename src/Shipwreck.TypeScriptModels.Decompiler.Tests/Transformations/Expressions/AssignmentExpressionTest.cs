using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Expressions
{
    [TestClass]
    public class AssignmentExpressionTest
    {
        private class TestClass
        {
            public int SourceMethod(int a)
            {
                a = a + a;
                return a -= 5;
            }
        }

        [TestMethod]
        public void AssignmentExpression_Test()
        {
            var t = new TypeTranslationContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}