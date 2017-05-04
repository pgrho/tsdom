using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shipwreck.TypeScriptModels.Declarations;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Members
{
    public abstract class PropertydDeclarationTestClass
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

    [TestClass]
    public class PropertydDeclarationTest : TransformationTestBase<PropertydDeclarationTestClass>
    {
        [TestMethod]
        public void PropertydDeclaration_AutoInt32PropertyTest()
        {
            var f = GetField(nameof(PropertydDeclarationTestClass.AutoInt32Property));

            Assert.AreEqual(AccessibilityModifier.Public, f.Accessibility);
            Assert.AreEqual(PredefinedType.Number, f.FieldType);
        }

        [TestMethod]
        public void PropertydDeclaration_ReadOnlyInt32PropertyTest()
        {
            var f = GetField("__ReadOnlyInt32Property");

            Assert.AreEqual(AccessibilityModifier.Private, f.Accessibility);
            Assert.AreEqual(PredefinedType.Number, f.FieldType);

            var g = GetGetAccessor("ReadOnlyInt32Property");
            Assert.AreEqual(AccessibilityModifier.Protected, g.Accessibility);
            Assert.AreEqual(PredefinedType.Number, g.PropertyType);

            var s = GetSetAccessor("ReadOnlyInt32Property");
            Assert.IsNull(s);
        }

        [TestMethod]
        public void PropertydDeclaration_WriteOnlyInt32PropertyTest()
        {
            var f = GetField("__WriteOnlyInt32Property");

            Assert.AreEqual(AccessibilityModifier.Private, f.Accessibility);
            Assert.AreEqual(PredefinedType.Number, f.FieldType);

            var g = GetGetAccessor("WriteOnlyInt32Property");
            Assert.IsNull(g);

            var s = GetSetAccessor("WriteOnlyInt32Property");
            Assert.AreEqual(AccessibilityModifier.Protected, s.Accessibility);
            Assert.AreEqual(PredefinedType.Number, s.PropertyType);
            Assert.AreEqual("value", s.ParameterName);
        }

        [TestMethod]
        public void PropertydDeclaration_AccessibilityInt32PropertyTest()
        {
            var f = GetField("__" + nameof(PropertydDeclarationTestClass.AccessibilityInt32Property));

            Assert.AreEqual(AccessibilityModifier.Private, f.Accessibility);
            Assert.AreEqual(PredefinedType.Number, f.FieldType);

            var g = GetGetAccessor(nameof(PropertydDeclarationTestClass.AccessibilityInt32Property));
            Assert.AreEqual(AccessibilityModifier.Public, g.Accessibility);
            Assert.AreEqual(PredefinedType.Number, g.PropertyType);

            var s = GetSetAccessor(nameof(PropertydDeclarationTestClass.AccessibilityInt32Property));
            Assert.AreEqual(AccessibilityModifier.Private, s.Accessibility);
            Assert.AreEqual(PredefinedType.Number, s.PropertyType);
            Assert.AreEqual("value", s.ParameterName);
        }
    }
}
