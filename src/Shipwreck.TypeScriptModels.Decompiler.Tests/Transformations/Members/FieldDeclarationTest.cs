using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Members
{
    public class FieldDeclarationTestClass
    {
        protected int _IntegerField;
    }

    [TestClass]
    public class FieldDeclarationTest : TransformationTestBase<FieldDeclarationTestClass>
    {
        [TestMethod]
        public void TransformTest()
        {
            var f = GetField("_IntegerField");

            Assert.IsNotNull(f);
        }
    }
}
