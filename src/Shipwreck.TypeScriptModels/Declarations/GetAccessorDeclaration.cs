using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class GetAccessorDeclaration : AccessorDeclaration
    {
        internal override bool GetIsSet() => false;

        public override void Accept<T>(IClassMemberVisitor<T> visitor)
            => visitor.VisitGetAccessor(this);

        public override void Accept<T>(IInterfaceMemberVisitor<T> visitor)
            => visitor.VisitGetAccessor(this);
    }
}