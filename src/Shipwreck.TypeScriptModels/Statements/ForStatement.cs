namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.5
    public sealed class ForStatement : Statement, IStatementOwner
    {
        private StatementCollection _Statements;

        public Expression Initializer { get; set; }
        public Expression Condition { get; set; }
        public Expression Iterator { get; set; }

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
            => visitor.VisitFor(this);
    }
}