namespace Shipwreck.TypeScriptModels.Statements
{

    // 5.7
    public sealed class ForOfStatement : Statement
    {
        public override T Accept<T>(IStatementVistor<T> visitor)
            => visitor.VisitForOf(this);
    }
}