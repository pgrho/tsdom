using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shipwreck.TypeScriptModels.Declarations;
using System;
using System.ComponentModel;
using System.Linq;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    [Serializable]
    [DefaultProperty("property")]
    public abstract class PublicClass
    {
        public PublicClass(float n, object obj)
        {
        }

        public int AutoInt32Property { get; set; }
        protected abstract int ReadOnlyInt32Property { get; }
        protected abstract int WriteOnlyInt32Property { set; }
        internal int AccessibilityInt32Property { get; private set; }

        public int ManualInt32Property
        {
            get { return 0; }
            set { }
        }

        private void Method1(bool b, int n, string s)
        {
        }
    }

    [TestClass]
    public class ClrToTypeScriptTransformerTest
    {
        private ClassDeclaration _PublicClass;

        public ClassDeclaration PC
        {
            get
            {
                if (_PublicClass == null)
                {
                    var clr2ts = new ClrToTypeScriptTransformer();
                    clr2ts.Transform(typeof(PublicClass));

                    foreach (var m in clr2ts.Statements)
                    {
                        Console.WriteLine(m);
                    }

                    _PublicClass = clr2ts.Statements.Cast<NamespaceDeclaration>().Single().Members.OfType<ClassDeclaration>().Single();

                    Assert.AreEqual(typeof(PublicClass).Name, _PublicClass.Name);
                }
                return _PublicClass;
            }
        }

        [TestMethod]
        public void AutoInt32PropertyTest()
        {
            var f = PC.Members.OfType<FieldDeclaration>().Single(e => e.FieldName == nameof(PublicClass.AutoInt32Property));

            Assert.AreEqual(AccessibilityModifier.Public, f.Accessibility);
            Assert.AreEqual(PredefinedType.Number, f.FieldType);
        }

        [TestMethod]
        public void ReadOnlyInt32PropertyTest()
        {
            var f = PC.Members.OfType<FieldDeclaration>().Single(e => e.FieldName == "__ReadOnlyInt32Property");

            Assert.AreEqual(AccessibilityModifier.Private, f.Accessibility);
            Assert.AreEqual(PredefinedType.Number, f.FieldType);

            var g = PC.Members.OfType<GetAccessorDeclaration>().Single(e => e.PropertyName == "ReadOnlyInt32Property");
            Assert.AreEqual(AccessibilityModifier.Protected, g.Accessibility);
            Assert.AreEqual(PredefinedType.Number, g.PropertyType);

            var s = PC.Members.OfType<SetAccessorDeclaration>().Any(e => e.PropertyName == "ReadOnlyInt32Property");
            Assert.IsFalse(s);
        }

        [TestMethod]
        public void WriteOnlyInt32PropertyTest()
        {
            var f = PC.Members.OfType<FieldDeclaration>().Single(e => e.FieldName == "__WriteOnlyInt32Property");

            Assert.AreEqual(AccessibilityModifier.Private, f.Accessibility);
            Assert.AreEqual(PredefinedType.Number, f.FieldType);

            var g = PC.Members.OfType<GetAccessorDeclaration>().Any(e => e.PropertyName == "WriteOnlyInt32Property");
            Assert.IsFalse(g);

            var s = PC.Members.OfType<SetAccessorDeclaration>().Single(e => e.PropertyName == "WriteOnlyInt32Property");
            Assert.AreEqual(AccessibilityModifier.Protected, s.Accessibility);
            Assert.AreEqual(PredefinedType.Number, s.PropertyType);
            Assert.AreEqual("value", s.ParameterName);
        }

        [TestMethod]
        public void AccessibilityInt32PropertyTest()
        {
            var f = PC.Members.OfType<FieldDeclaration>().Single(e => e.FieldName == "__" + nameof(PublicClass.AccessibilityInt32Property));

            Assert.AreEqual(AccessibilityModifier.Private, f.Accessibility);
            Assert.AreEqual(PredefinedType.Number, f.FieldType);

            var g = PC.Members.OfType<GetAccessorDeclaration>().Single(e => e.PropertyName == nameof(PublicClass.AccessibilityInt32Property));
            Assert.AreEqual(AccessibilityModifier.Public, g.Accessibility);
            Assert.AreEqual(PredefinedType.Number, g.PropertyType);

            var s = PC.Members.OfType<SetAccessorDeclaration>().Single(e => e.PropertyName == nameof(PublicClass.AccessibilityInt32Property));
            Assert.AreEqual(AccessibilityModifier.Private, s.Accessibility);
            Assert.AreEqual(PredefinedType.Number, s.PropertyType);
            Assert.AreEqual("value", s.ParameterName);
        }
    }
}