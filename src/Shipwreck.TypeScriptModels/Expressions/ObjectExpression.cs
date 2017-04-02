using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    // 4.5
    public sealed class ObjectExpression : Expression
    {
        private Collection<IObjectLiteralMember> _Members;

        public bool HasMember
            => _Members?.Count > 0;

        public Collection<IObjectLiteralMember> Members
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Members);
            }
            set
            {
                CollectionHelper.Set(ref _Members, value);
            }
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitObject(this);
    }
}