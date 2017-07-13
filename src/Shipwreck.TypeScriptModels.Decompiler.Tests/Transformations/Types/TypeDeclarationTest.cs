using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Types
{
    public sealed class InheritedTypeDeclarationTestClass : ArrayList
    {
    }
    public sealed class ImplementedTypeDeclarationTestClass : IDisposable
    {
        public void Dispose() { }
    }
    public sealed class InheritedAndImplementedTypeDeclarationTestClass : ArrayList, IDisposable
    {
        public void Dispose() { }
    }
    [TestClass]
    public sealed class TypeDeclarationTest
    {
        [TestMethod]
        public void InheritedTypeDeclaration_TransformTest()
        {
            var c = new TransformationTestBase<InheritedTypeDeclarationTestClass>().Result;
            Assert.IsNotNull(c);
        }
        [TestMethod]
        public void ImplementedTypeDeclaration_TransformTest()
        {
            var c = new TransformationTestBase<ImplementedTypeDeclarationTestClass>().Result;
            Assert.IsNotNull(c);
        }
        [TestMethod]
        public void InheritedAndImplementedTypeDeclaration_TransformTest()
        {
            var c = new TransformationTestBase<InheritedAndImplementedTypeDeclarationTestClass>().Result;
            Assert.IsNotNull(c);
        }
    }
}
