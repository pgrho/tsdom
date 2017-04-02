using System.Collections.ObjectModel;

namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.6
    public sealed class ForInStatement : Statement
    {
        private Collection<Statement> _Statements;

        public Expression Variable { get; set; }
        public Expression Value { get; set; }

        public bool HasStatement
            => _Statements?.Count > 0;

        public Collection<Statement> Statements
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Statements);
            }
            set
            {
                CollectionHelper.Set(ref _Statements, value);
            }
        }

        public override T Accept<T>(IStatementVistor<T> visitor)
            => visitor.VisitForIn(this);
    }
}