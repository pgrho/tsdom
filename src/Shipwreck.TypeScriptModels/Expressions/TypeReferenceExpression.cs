namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class TypeReferenceExpression : Expression
    {
        public TypeReferenceExpression()
        {
        }

        public TypeReferenceExpression(ITypeReference type)
        {
            Type = type;
        }

        public ITypeReference Type { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitTypeReference(this);
    }
}