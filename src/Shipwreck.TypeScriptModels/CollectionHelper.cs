using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels
{
    internal static class CollectionHelper
    {
        public static Collection<T> GetOrCreate<T>(ref Collection<T> field)
            => field ?? (field = new Collection<T>());

        public static void Set<T>(ref Collection<T> field, Collection<T> value)
        {
            if (value != field)
            {
                field?.Clear();
                if (value?.Count > 0)
                {
                    var ta = GetOrCreate(ref field);
                    foreach (var v in value)
                    {
                        ta.Add(v);
                    }
                }
            }
        }
    }
}
