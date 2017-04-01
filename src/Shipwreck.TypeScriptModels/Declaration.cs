using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public abstract class Declaration : Statement
    {
        public string Name { get; set; }

        public Documentation Documentation { get; set; }
    }
}