using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shipwreck.TypeScriptModels.Declarations;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Members
{
    [TestClass]
    public class PropertydDeclarationTest
    {
        private abstract class TestClass
        {
            public int AutoInt32Property { get; set; }
            protected abstract int ReadOnlyInt32Property { get; }
            protected abstract int WriteOnlyInt32Property { set; }
            internal int AccessibilityInt32Property { get; private set; }

            public int ManualInt32Property
            {
                get { return 0; }
                set { }
            }
        }

        [TestMethod]
        public void PropertydDeclaration_AutoInt32PropertyTest()
        {
            var t = new TypeTranslationContext<TestClass>();
            var f = t.GetField(nameof(TestClass.AutoInt32Property));

            Assert.AreEqual(AccessibilityModifier.Public, f.Accessibility);
            Assert.AreEqual(PredefinedType.Number, f.FieldType);
        }

        [TestMethod]
        public void PropertydDeclaration_ReadOnlyInt32PropertyTest()
        {
            var t = new TypeTranslationContext<TestClass>();
            var f = t.GetField("__ReadOnlyInt32Property");

            Assert.AreEqual(AccessibilityModifier.Private, f.Accessibility);
            Assert.AreEqual(PredefinedType.Number, f.FieldType);

            var g = t.GetGetAccessor("ReadOnlyInt32Property");
            Assert.AreEqual(AccessibilityModifier.Protected, g.Accessibility);
            Assert.AreEqual(PredefinedType.Number, g.PropertyType);

            var s = t.GetSetAccessor("ReadOnlyInt32Property");
            Assert.IsNull(s);
        }

        [TestMethod]
        public void PropertydDeclaration_WriteOnlyInt32PropertyTest()
        {
            var t = new TypeTranslationContext<TestClass>();
            var f = t.GetField("__WriteOnlyInt32Property");

            Assert.AreEqual(AccessibilityModifier.Private, f.Accessibility);
            Assert.AreEqual(PredefinedType.Number, f.FieldType);

            var g = t.GetGetAccessor("WriteOnlyInt32Property");
            Assert.IsNull(g);

            var s = t.GetSetAccessor("WriteOnlyInt32Property");
            Assert.AreEqual(AccessibilityModifier.Protected, s.Accessibility);
            Assert.AreEqual(PredefinedType.Number, s.PropertyType);
            Assert.AreEqual("value", s.ParameterName);
        }

        [TestMethod]
        public void PropertydDeclaration_AccessibilityInt32PropertyTest()
        {
            var t = new TypeTranslationContext<TestClass>();
            var f = t.GetField("__" + nameof(TestClass.AccessibilityInt32Property));

            Assert.AreEqual(AccessibilityModifier.Private, f.Accessibility);
            Assert.AreEqual(PredefinedType.Number, f.FieldType);

            var g = t.GetGetAccessor(nameof(TestClass.AccessibilityInt32Property));
            Assert.AreEqual(AccessibilityModifier.Public, g.Accessibility);
            Assert.AreEqual(PredefinedType.Number, g.PropertyType);

            var s = t.GetSetAccessor(nameof(TestClass.AccessibilityInt32Property));
            Assert.AreEqual(AccessibilityModifier.Private, s.Accessibility);
            Assert.AreEqual(PredefinedType.Number, s.PropertyType);
            Assert.AreEqual("value", s.ParameterName);
        }
    }
}