using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Shipwreck.TypeScriptModels
{
    public sealed class StatementCollection : OwnedCollection<Statement, IStatementOwner>
    {
        public StatementCollection()
           : base(null)
        {
        }

        internal StatementCollection(IStatementOwner owner)
            : base(owner)
        {
        }

        public Statement Add(Expression expression)
        {
            var s = expression.ToStatement();
            Add(s);
            return s;
        }
    }
}