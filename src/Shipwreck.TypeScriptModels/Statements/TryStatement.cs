using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.14
    public sealed class TryStatement : Statement
    {
        public Expression CatchParameter { get; set; }

        private Collection<Statement> _TryBlock;

        public bool HasTryBlock
            => _TryBlock?.Count > 0;

        public Collection<Statement> TryBlock
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _TryBlock);
            }
            set
            {
                CollectionHelper.Set(ref _TryBlock, value);
            }
        }

        private Collection<Statement> _CatchBlock;

        public bool HasCatchBlock
            => _CatchBlock?.Count > 0;

        public Collection<Statement> CatchBlock
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _CatchBlock);
            }
            set
            {
                CollectionHelper.Set(ref _CatchBlock, value);
            }
        }

        private Collection<Statement> _FinallyBlock;

        public bool HasFinallyBlock
            => _FinallyBlock?.Count > 0;

        public Collection<Statement> FinallyBlock
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _FinallyBlock);
            }
            set
            {
                CollectionHelper.Set(ref _FinallyBlock, value);
            }
        }

        public override T Accept<T>(IStatementVistor<T> visitor)
            => visitor.VisitTry(this);
    }
}