using System.Collections.ObjectModel;

namespace Shipwreck.TypeScriptModels
{
    internal static class CollectionHelper
    {
        public static Collection<T> GetOrCreate<T>(ref Collection<T> field)
            => field ?? (field = new Collection<T>());

        public static OwnedCollection<TItem, TOwner> GetOrCreate<TItem, TOwner>(ref OwnedCollection<TItem, TOwner> field, TOwner owner)
            where TItem : class, IHasParent
            where TOwner : class
            => field ?? (field = new OwnedCollection<TItem, TOwner>(owner));

        public static StatementCollection GetOrCreate(ref StatementCollection field, IStatementOwner owner)
            => field ?? (field = new StatementCollection(owner));

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

        public static void Set<TItem, TOwner>(ref OwnedCollection<TItem, TOwner> field, Collection<TItem> value, TOwner owner)
            where TItem : class, IHasParent
            where TOwner : class
        {
            if (value != field)
            {
                field?.Clear();
                if (value?.Count > 0)
                {
                    var ta = GetOrCreate(ref field, owner);
                    foreach (var v in value)
                    {
                        ta.Add(v);
                    }
                }
            }
        }

        public static void Set(ref StatementCollection field, Collection<Statement> value, IStatementOwner owner)
        {
            if (value != field)
            {
                field?.Clear();
                if (value?.Count > 0)
                {
                    var ta = GetOrCreate(ref field, owner);
                    foreach (var v in value)
                    {
                        ta.Add(v);
                    }
                }
            }
        }
    }
}