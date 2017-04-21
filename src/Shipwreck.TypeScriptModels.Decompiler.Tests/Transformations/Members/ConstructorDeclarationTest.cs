using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Members
{
    public class ConstructorDeclarationTestClass
    {
        public ConstructorDeclarationTestClass(float n, object obj)
        {
        }
    }

    [TestClass]
    public class ConstructorDeclarationTest : TransformationTestBase<ConstructorDeclarationTestClass>
    {
        [TestMethod]
        public void TransformTest()
        {
            var f = GetConstructor();

            Assert.IsNotNull(f);
        }
    }
}
