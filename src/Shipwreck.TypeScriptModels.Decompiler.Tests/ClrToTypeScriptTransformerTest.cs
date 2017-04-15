using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    [Serializable]
    [DefaultProperty("property")]
    public abstract class PublicClass
    {
        public int AutoInt32Property { get; set; }
        protected abstract int ReadOnlyInt32Property { get; }
        protected abstract int WriteOnlyInt32Property { set; }
        internal int AccessibilityInt32Property { get; private set; }
        public int ManualInt32Property
        {
            get { return 0; }
            set { }
        }
    }

    [TestClass]
    public class ClrToTypeScriptTransformerTest
    {
        [TestMethod]
        public void TransformTest()
        {
            var clr2ts = new ClrToTypeScriptTransformer();
            clr2ts.Transform(typeof(PublicClass));

            foreach (var m in clr2ts.Statements)
            {
                Console.WriteLine(m);
            }
        }
    }
}
