namespace Shipwreck.TypeScriptModels.Expressions
{
    // 4.4
    public sealed class BooleanExpression : Expression
    {
        public BooleanExpression()
        {
        }

        public BooleanExpression(bool value)
        {
            Value = value;
        }

        public override ExpressionPrecedence Precedence
            => ExpressionPrecedence.Grouping;

        public bool Value { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitBoolean(this);
    }
}