using System.Collections;

namespace Shipwreck.TypeScriptModels
{
    public interface IOwnedCollection : IList
    {
        object Owner { get; }
    }
}