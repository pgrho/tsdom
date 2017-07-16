using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Members
{
    [TestClass]
    public class FieldDeclarationTest
    {
        private class TestClass
        {
#pragma warning disable CS0649
            protected int _IntegerField;
#pragma warning restore CS0649
        }

        [TestMethod]
        public void FieldDeclaration_Test()
        {
            var f = new TypeTranslationContext<TestClass>().GetField("_IntegerField");

            Assert.IsNotNull(f);
        }
    }
}