using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public sealed class MethodDeclaration : MemberDeclaration, ITypeScriptFunction
    {
        private List<ParameterDeclaration> _Parameters;

        public ITypeScriptType ReturnType { get; set; }

        public List<ParameterDeclaration> Parameters
        {
            get
            {
                return _Parameters ?? (_Parameters = new List<ParameterDeclaration>());
            }
            set
            {
                _Parameters?.Clear();
                if (value?.Count > 0)
                {
                    Parameters.AddRange(value);
                }
            }
        }

        public override void WriteAsDeclaration(IndentedTextWriter writer)
        {
            if (AccessModifier != AccessModifier.Public)
            {
                return;
            }

            writer.WriteDocumentation(Documentation);
            writer.Write(Name);
            writer.WriteParameterDeclaration(this, false);
        }
    }
}