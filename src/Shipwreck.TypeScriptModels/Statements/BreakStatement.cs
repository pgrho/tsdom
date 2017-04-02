namespace Shipwreck.TypeScriptModels.Statements
{

    // 5.9
    public sealed class BreakStatement : Statement
    {
        public override T Accept<T>(IStatementVistor<T> visitor)
            => visitor.VisitBreak(this);
    }
}