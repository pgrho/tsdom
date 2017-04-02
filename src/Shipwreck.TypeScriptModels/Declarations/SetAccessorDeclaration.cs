using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class SetAccessorDeclaration : AccessorDeclaration
    {
        public string ParameterName { get; set; }

        internal override bool GetIsSet() => true;
        public override void Accept<T>(IObjectLiteralVisitor<T> visitor)
            => visitor.VisitSetAccessor(this);
    }
}
