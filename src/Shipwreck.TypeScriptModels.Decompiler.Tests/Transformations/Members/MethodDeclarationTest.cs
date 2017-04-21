using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Members
{
    public class MethodDeclarationTestClass
    {
        private void Method1(bool b, int n, string s)
        {
        }

        public static void GenericMethod<T1, T2>()
            where T1 : class, IConvertible, new()
            where T2 : MethodDeclarationTestClass
        {
        }
    }

    [TestClass]
    public class MethodDeclarationTest : TransformationTestBase<MethodDeclarationTestClass>
    {
        [TestMethod]
        public void TransformTest()
        {
            var f = GetMethod("Method1");

            Assert.IsNotNull(f);
        }
    }
}
