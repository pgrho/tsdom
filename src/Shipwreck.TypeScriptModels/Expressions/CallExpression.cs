using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    // 4.15
    public sealed class CallExpression : Expression
    {
        public override ExpressionPrecedence Precedence
            => ExpressionPrecedence.FunctionCall;

        public Expression Type { get; set; }

        private Collection<ITypeReference> _TypeArguments;

        public bool HasTypeArgument
            => _TypeArguments?.Count > 0;

        public Collection<ITypeReference> TypeArguments
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _TypeArguments);
            }
            set
            {
                CollectionHelper.Set(ref _TypeArguments, value);
            }
        }

        private Collection<Expression> _Parameters;

        public bool HasParameter
            => _Parameters?.Count > 0;

        public Collection<Expression> Parameters
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Parameters);
            }
            set
            {
                CollectionHelper.Set(ref _Parameters, value);
            }
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitCall(this);
    }
}
