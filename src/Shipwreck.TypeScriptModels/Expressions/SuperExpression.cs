namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class SuperExpression : Expression
    {
        /// <inheritdoc />
        /// <summary>
        /// This method always calls <see cref="IExpressionVisitor{T}.VisitSuper" />.
        /// </summary>
        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitSuper();
    }
}
