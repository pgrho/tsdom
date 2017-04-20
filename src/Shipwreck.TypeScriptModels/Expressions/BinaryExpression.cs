namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class BinaryExpression : Expression
    {
        public override ExpressionPrecedence Precedence
        {
            get
            {
                switch (Operator)
                {
                    case BinaryOperator.Add:
                        return ExpressionPrecedence.Add;

                    case BinaryOperator.Subtract:
                        return ExpressionPrecedence.Subtract;

                    case BinaryOperator.Multiply:
                        return ExpressionPrecedence.Multiply;

                    case BinaryOperator.Divide:
                    case BinaryOperator.IntegerDivide:
                        return ExpressionPrecedence.Divide;

                    case BinaryOperator.Modulo:
                        return ExpressionPrecedence.Modulo;

                    case BinaryOperator.LeftShift:
                        return ExpressionPrecedence.LeftShift;

                    case BinaryOperator.SignedRightShift:
                        return ExpressionPrecedence.RightShift;

                    case BinaryOperator.UnsignedRightShift:
                        return ExpressionPrecedence.UnsignedRightShift;

                    case BinaryOperator.BitwiseAnd:
                        return ExpressionPrecedence.BitwiseAnd;

                    case BinaryOperator.BitwiseOr:
                        return ExpressionPrecedence.BitwiseOr;

                    case BinaryOperator.BitwiseXor:
                        return ExpressionPrecedence.BitwiseXor;

                    case BinaryOperator.Exponent:
                        return ExpressionPrecedence.Exponent;

                    case BinaryOperator.Equal:
                        return ExpressionPrecedence.Equal;

                    case BinaryOperator.NotEqual:
                        return ExpressionPrecedence.NotEqual;

                    case BinaryOperator.StrictEqual:
                        return ExpressionPrecedence.StrictEqual;

                    case BinaryOperator.StrictNotEqual:
                        return ExpressionPrecedence.StrictNotEqual;

                    case BinaryOperator.LessThan:
                        return ExpressionPrecedence.LessThan;

                    case BinaryOperator.LessThanOrEqual:
                        return ExpressionPrecedence.LessThanOrEqual;

                    case BinaryOperator.GreaterThan:
                        return ExpressionPrecedence.GreaterThan;

                    case BinaryOperator.GreaterThanOrEqual:
                        return ExpressionPrecedence.GreaterThanOrEqual;

                    case BinaryOperator.InstanceOf:
                        return ExpressionPrecedence.InstanceOf;

                    case BinaryOperator.In:
                        return ExpressionPrecedence.In;

                    case BinaryOperator.LogicalAnd:
                        return ExpressionPrecedence.LogicalAnd;

                    case BinaryOperator.LogicalOr:
                        return ExpressionPrecedence.LogicalOr;
                }

                return ExpressionPrecedence.Unknown;
            }
        }

        public BinaryOperator Operator { get; set; }
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitBinary(this);
    }
}