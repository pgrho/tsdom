using System.Collections.ObjectModel;

namespace Shipwreck.TypeScriptModels.Statements
{
    public sealed class BlockStatement : Statement
    {
        private Collection<Statement> _Statements;

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

        public override T Accept<T>(IStatementVisitor<T> visitor)
            => visitor.VisitBlock(this);
    }
}