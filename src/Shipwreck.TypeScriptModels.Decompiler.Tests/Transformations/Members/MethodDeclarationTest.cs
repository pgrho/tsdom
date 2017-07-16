using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Members
{
    [TestClass]
    public class MethodDeclarationTest
    {
        private class TestClass
        {
            private void Method1(bool b, int n, string s)
            {
            }

            public static void GenericMethod<T1, T2>()
                where T1 : class, IConvertible, new()
                where T2 : TestClass
            {
            }
        }

        [TestMethod]
        public void MethodDeclaration_Test()
        {
            var t = new TypeTranslationContext<TestClass>();
            var f = t.GetMethod("Method1");

            Assert.IsNotNull(f);
        }
    }
}