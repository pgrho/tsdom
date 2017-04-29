namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class AwaitExpression : Expression
    {
        // TODO: 要出典
        public override ExpressionPrecedence Precedence
            => ExpressionPrecedence.Conditional;

        public Expression Operand { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitAwait(this);
    }
}