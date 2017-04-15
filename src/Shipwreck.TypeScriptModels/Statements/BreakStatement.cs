namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.9
    public sealed class BreakStatement : Statement
    {
        public override T Accept<T>(IStatementVisitor<T> visitor)
            => visitor.VisitBreak(this);
    }
}