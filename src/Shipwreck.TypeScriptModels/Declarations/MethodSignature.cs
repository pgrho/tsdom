using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class MethodSignature : CallSignatureBase
    {
        public string MethodName { get; set; }
        public bool IsOptional { get; set; }

        internal override void WriteSignature(TextWriter writer)
        {
            writer.Write(MethodName);
            if (IsOptional)
            {
                writer.Write('?');
            }
            base.WriteSignature(writer);
        }
    }
}
