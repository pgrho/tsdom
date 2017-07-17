namespace Shipwreck.TypeScriptModels.Statements
{
    public sealed class BlockStatement : Statement, IStatementOwner
    {
        private StatementCollection _Statements;

        public bool HasStatement
            => _Statements?.Count > 0;

        public StatementCollection Statements
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Statements, this);
            }
            set
            {
                CollectionHelper.Set(ref _Statements, value, this);
            }
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
            => visitor.VisitBlock(this);
    }
}