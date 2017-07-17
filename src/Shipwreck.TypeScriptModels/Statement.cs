using System.Collections;

namespace Shipwreck.TypeScriptModels
{
    public abstract class Statement : Syntax, IHasParent, IHasParentInternal
    {
        /// <summary>
        /// Gets the list that the statement belongs to.
        /// </summary>
        public IList Parent { get; private set; }

        void IHasParentInternal.SetParent(IList value)
        {
            Parent = value;
        }

        public abstract T Accept<T>(IStatementVisitor<T> visitor);
    }
}