namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class NamespaceDeclaration : ModuleDeclarationBase<INamespaceMember>
    {
        /// <inheritdoc />
        public override void Accept<T>(IRootStatementVisitor<T> visitor)
            => visitor.VisitNamespaceDeclaration(this);
    }
}