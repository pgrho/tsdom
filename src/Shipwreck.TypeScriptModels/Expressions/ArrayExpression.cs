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
        public ArrayExpression() { }
        public ArrayExpression(IEnumerable<Expression> elements)
        {
            foreach (var e in elements)
            {
                Elements.Add(e);
            }
        }

        private Collection<Expression> _Elements;

        public override ExpressionPrecedence Precedence
            => ExpressionPrecedence.Grouping;

        public bool HasElement
            => _Elements?.Count > 0;



        public Collection<Expression> Elements
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
