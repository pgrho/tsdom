﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class ConstructorType : FunctionTypeBase
    {
        internal override void WritePrefix(TextWriter writer)
            => writer.Write("new ");
    }
}