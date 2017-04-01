using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    [Flags]
    public enum TypeFlags
    {
        None = 0,
        Required = 1,
        Nullable = 2,
    }
}