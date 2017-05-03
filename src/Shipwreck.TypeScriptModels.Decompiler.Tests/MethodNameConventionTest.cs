using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    class MethodNameConventionTestClass
    {
        public override string ToString()
        {
            return "hoge";
        }

        public string Test(int n)
            => n.ToString();
    }

    [TestClass]
    public class MethodNameConventionTest
    {
        [TestMethod]
        public void MethodNameConvention_DeclarationTest()
        {
            var t = new ILTranslator();
            var r = t.Transform(typeof(MethodNameConventionTestClass)).Single();
            Console.WriteLine(r);
        }

        [TestMethod]
        public void MethodNameConvention_InvocationTest()
        {
            var t = new ILTranslator();
            var r = t.Transform(typeof(MethodNameConventionTestClass)).Single();
            Console.WriteLine(r);
        }
    }
}
