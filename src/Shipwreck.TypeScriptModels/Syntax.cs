using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels
{
    /// <summary>
    /// Provides the base class from which the classes that represent syntax tree nodes are derived.
    /// </summary>
    public abstract class Syntax
    {
        internal Syntax() { }

        public override string ToString()
        {
            using (var w = new StringWriter())
            using (var tsw = new TypeScriptWriter(w))
            {
                tsw.Write(this);
                tsw.Flush();

                return w.ToString();
            }
        }
    }
}
