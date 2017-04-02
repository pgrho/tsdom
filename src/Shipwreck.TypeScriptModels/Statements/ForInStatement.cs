namespace Shipwreck.TypeScriptModels.Statements
{

    // 5.6
    public sealed class ForInStatement : Statement
    {
        public override T Accept<T>(IStatementVistor<T> visitor)
            => visitor.VisitForIn(this);
    }
}