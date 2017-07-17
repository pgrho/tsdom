using System.Collections;

namespace Shipwreck.TypeScriptModels
{
    internal interface IHasParentInternal : IHasParent
    {
        void SetParent(IList value);
    }
}