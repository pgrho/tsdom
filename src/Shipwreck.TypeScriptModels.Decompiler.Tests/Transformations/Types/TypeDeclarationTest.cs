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
        public void InheritedTypeDeclaration_TransformTest()
        {
            var c = new TransformingContext<InheritedTestClass>().Result;
            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void ImplementedTypeDeclaration_TransformTest()
        {
            var c = new TransformingContext<ImplementedTestClass>().Result;
            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void InheritedAndImplementedTypeDeclaration_TransformTest()
        {
            var c = new TransformingContext<InheritedAndImplementedTestClass>().Result;
            Assert.IsNotNull(c);
        }
    }
}