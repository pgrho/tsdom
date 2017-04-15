namespace Shipwreck.TypeScriptModels.Statements
{

    // 5.8
    public sealed class ContinueStatement : Statement
    {
        public override T Accept<T>(IStatementVisitor<T> visitor)
            => visitor.VisitContinue(this);
    }
}