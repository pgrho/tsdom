using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class ArrayExpression : Expression
    {

        private Collection<IObjectLiteralMember> _Elements;

        public bool HasElement
            => _Elements?.Count > 0;

        public Collection<IObjectLiteralMember> Elements
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Elements);
            }
            set
            {
                CollectionHelper.Set(ref _Elements, value);
            }
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitArray(this);
    }
}
