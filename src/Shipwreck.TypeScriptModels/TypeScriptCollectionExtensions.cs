using System.Collections.Generic;

namespace Shipwreck.TypeScriptModels
{

    public static class TypeScriptCollectionExtensions
    {
        public static void AddRange<T>(this TypeScriptCollection<T> collection, IEnumerable<T> values)
            where T : Statement
        {
            foreach (var v in values)
            {
                collection.Add(v);
            }
        }
    }
}
