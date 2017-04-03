namespace Shipwreck.TypeScriptModels.Expressions
{
    // 4.2
    public sealed class ThisExpression : Expression
    {
        /// <inheritdoc />
        /// <summary>
        /// This method always calls <see cref="IExpressionVisitor{T}.VisitThis" />.
        /// </summary>
        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitThis();
    }
}