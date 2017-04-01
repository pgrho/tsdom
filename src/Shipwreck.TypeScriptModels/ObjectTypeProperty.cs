using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public class ObjectTypeProperty
    {
        public string Name { get; set; }

        public ITypeScriptType PropertyType { get; set; }
        public bool IsRequired { get; set; }
    }
}