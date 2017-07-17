﻿namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.7
    public sealed class ForOfStatement : Statement, IStatementOwner
    {
        private StatementCollection _Statements;

        public Expression Variable { get; set; }
        public Expression Value { get; set; }

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
            => visitor.VisitForOf(this);
    }
}