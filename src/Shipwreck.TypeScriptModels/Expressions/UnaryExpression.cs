namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class UnaryExpression : Expression
    {
        public override ExpressionPrecedence Precedence
        {
            get
            {
                switch (Operator)
                {
                    case UnaryOperator.PrefixIncrement:
                        return ExpressionPrecedence.PrefixIncrement;

                    case UnaryOperator.PrefixDecrement:
                        return ExpressionPrecedence.PrefixDecrement;

                    case UnaryOperator.PostfixIncrement:
                        return ExpressionPrecedence.PostfixIncrement;

                    case UnaryOperator.PostfixDecrement:
                        return ExpressionPrecedence.PostfixDecrement;

                    case UnaryOperator.Plus:
                        return ExpressionPrecedence.UnaryPlus;

                    case UnaryOperator.Minus:
                        return ExpressionPrecedence.UnaryNegation;

                    case UnaryOperator.BitwiseNot:
                        return ExpressionPrecedence.BitwiseNot;

                    case UnaryOperator.LogicalNot:
                        return ExpressionPrecedence.LogicalNot;

                    case UnaryOperator.Delete:
                        return ExpressionPrecedence.Delete;

                    case UnaryOperator.Void:
                        return ExpressionPrecedence.Void;

                    case UnaryOperator.TypeOf:
                        return ExpressionPrecedence.TypeOf;
                }
                return ExpressionPrecedence.Unknown;
            }
        }

        public UnaryOperator Operator { get; set; }
        public Expression Operand { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitUnary(this);
    }
}