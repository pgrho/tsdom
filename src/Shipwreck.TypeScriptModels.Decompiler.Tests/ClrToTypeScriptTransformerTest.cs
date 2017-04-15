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
    public sealed class PublicClass
    {

    }

    [TestClass]
    public class ClrToTypeScriptTransformerTest
    {
        [TestMethod]
        public void TransformTest()
        {
            var clr2ts = new ClrToTypeScriptTransformer();
            clr2ts.Transform(typeof(PublicClass));
        }
    }
}
