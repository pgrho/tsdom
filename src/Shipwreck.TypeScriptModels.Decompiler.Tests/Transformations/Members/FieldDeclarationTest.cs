using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Members
{
    [TestClass]
    public class FieldDeclarationTest
    {
        private class TestClass
        {
            protected int _IntegerField;
        }

        [TestMethod]
        public void FieldDeclaration_TransformTest()
        {
            var f = new TransformingContext<TestClass>().GetField("_IntegerField");

            Assert.IsNotNull(f);
        }
    }
}