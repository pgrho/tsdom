using System.Collections;

namespace Shipwreck.TypeScriptModels
{
    public interface IHasParent
    {
        /// <summary>
        /// Gets the list that the statement belongs to.
        /// </summary>
        IOwnedCollection Parent { get; }
    }
}