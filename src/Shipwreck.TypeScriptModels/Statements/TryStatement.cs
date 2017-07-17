namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.14
    public sealed class TryStatement : Statement, IStatementOwner
    {
        public Expression CatchParameter { get; set; }

        private StatementCollection _TryBlock;

        public bool HasTryBlock
            => _TryBlock?.Count > 0;

        public StatementCollection TryBlock
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _TryBlock, this);
            }
            set
            {
                CollectionHelper.Set(ref _TryBlock, value, this);
            }
        }

        private StatementCollection _CatchBlock;

        public bool HasCatchBlock
            => _CatchBlock?.Count > 0;

        public StatementCollection CatchBlock
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _CatchBlock, this);
            }
            set
            {
                CollectionHelper.Set(ref _CatchBlock, value, this);
            }
        }

        private StatementCollection _FinallyBlock;

        public bool HasFinallyBlock
            => _FinallyBlock?.Count > 0;

        public StatementCollection FinallyBlock
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _FinallyBlock, this);
            }
            set
            {
                CollectionHelper.Set(ref _FinallyBlock, value, this);
            }
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
            => visitor.VisitTry(this);
    }
}