namespace Shipwreck.TypeScriptModels.Statements
{

    // 5.5
    public sealed class ForStatement : Statement
    {
        public override T Accept<T>(IStatementVistor<T> visitor)
            => visitor.VisitFor(this);
    }
}