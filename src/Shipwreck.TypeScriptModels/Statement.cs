using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public abstract class Statement
    {
        internal ITypeScriptObjectOwner Owner { get; set; }

        public abstract void WriteAsDeclaration(IndentedTextWriter writer);
    }
}