namespace Shipwreck.TypeScriptModels.Statements
{

    // 5.3
    public sealed class ConstDeclaration : LexicalDeclaration
    {
        public override T Accept<T>(IStatementVistor<T> visitor)
            => visitor.VisitConstDeclaration(this);
    }
}