using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Types
{
    [TestClass]
    public sealed class TypeDeclarationTest
    {
        private sealed class InheritedTestClass : ArrayList
        {
        }

        private sealed class ImplementedTestClass : IDisposable
        {
            public void Dispose()
            {
            }
        }

        private sealed class InheritedAndImplementedTestClass : ArrayList, IDisposable
        {
            public void Dispose()
            {
            }
        }

        [TestMethod]
        public void TypeDeclarationTest_Inherited()
        {
            var c = new TypeTranslationContext<InheritedTestClass>().Result;
            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void TypeDeclarationTest_Implemented()
        {
            var c = new TypeTranslationContext<ImplementedTestClass>().Result;
            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void TypeDeclarationTest_InheritedAndImplemented()
        {
            var c = new TypeTranslationContext<InheritedAndImplementedTestClass>().Result;
            Assert.IsNotNull(c);
        }
    }
}