using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Members
{
    [TestClass]
    public class ConstructorDeclarationTest
    {
        private class TestClass
        {
            public TestClass(float n, object obj)
            {
            }
        }

        [TestMethod]
        public void ConstructorDeclaration_Test()
        {
            var f = new TypeTranslationContext<TestClass>().GetConstructor();

            Assert.IsNotNull(f);
        }
    }
}