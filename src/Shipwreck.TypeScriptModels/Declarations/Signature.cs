using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public abstract class Signature
    {
        internal abstract void WriteSignature(TextWriter writer);
    }
}
