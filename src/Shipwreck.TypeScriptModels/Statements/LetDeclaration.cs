namespace Shipwreck.TypeScriptModels.Statements
{

    // 5.3
    public sealed class LetDeclaration : LexicalDeclaration
    {
        public override T Accept<T>(IStatementVistor<T> visitor)
            => visitor.VisitLetDeclaration(this);
    }
}