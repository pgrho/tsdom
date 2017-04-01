using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public sealed class ParameterDeclaration
    {
        public string Name { get; set; }

        public bool IsOptional { get; set; }

        public ITypeScriptType ParameterType { get; set; }
        public bool IsRequired { get; set; }
    }
}