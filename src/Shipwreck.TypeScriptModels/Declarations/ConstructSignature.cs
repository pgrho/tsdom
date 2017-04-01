using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class ConstructSignature : CallSignatureBase
    {
        internal override void WriteSignature(TextWriter writer)
        {
            writer.Write("new ");
            base.WriteSignature(writer);
        }
    }
}
