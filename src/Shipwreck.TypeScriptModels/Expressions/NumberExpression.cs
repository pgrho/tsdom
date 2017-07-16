namespace Shipwreck.TypeScriptModels.Expressions
{
    // 4.4
    public sealed class NumberExpression : Expression
    {
        public NumberExpression()
        {
        }

        public NumberExpression(double value)
        {
            Value = value;
        }

        public override ExpressionPrecedence Precedence
            => ExpressionPrecedence.Grouping;

        public double Value { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitNumber(this);
    }
}