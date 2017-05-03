using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    class MathConventionSetTestClass
    {
        public int Abs(int v)
            => Math.Abs(v);
    }
    [TestClass]
    public class MathConventionSetTest
    {
        [TestMethod]
        public void MathConventionSet_Test()
        {
            var t = new ILTranslator();
            var r = t.Transform(typeof(MathConventionSetTestClass)).Single();
            Console.WriteLine(r);
        }
    }
}
