using System.Collections.ObjectModel;

namespace Shipwreck.TypeScriptModels.Expressions
{
    // 4.14
    public sealed class NewExpression : Expression
    {
        public NewExpression()
        {
        }

        public NewExpression(Expression type)
        {
            Type = type;
        }

        public NewExpression(Expression type, params Expression[] parameters)
        {
            Type = type;
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    Parameters.Add(p);
                }
            }
        }

        public override ExpressionPrecedence Precedence
            => ExpressionPrecedence.NewWithArguments;

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
            => visitor.VisitNew(this);
    }
}