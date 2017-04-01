using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public sealed class ConstructorDeclaration : MemberDeclaration, ITypeScriptFunction
    {
        private List<ParameterDeclaration> _Parameters;

        ITypeScriptType ITypeScriptFunction.ReturnType { get { return DeclaringType; } set { } }
         
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
            if (DeclaringType != null)
            {
                writer.WriteDocumentation(Documentation);
                writer.Write("new "); 
                writer.WriteParameterDeclaration(this, false);
            }
        }
    }

}