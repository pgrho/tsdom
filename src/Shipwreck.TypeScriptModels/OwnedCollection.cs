using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Shipwreck.TypeScriptModels
{
    public class OwnedCollection<TItem, TOwner> : Collection<TItem>, IOwnedCollection
        where TItem : class, IHasParent
        where TOwner : class
    {
        [NonSerialized]
        private readonly WeakReference<TOwner> _Owner;

        internal OwnedCollection(TOwner owner)
        {
            _Owner = owner == null ? null : new WeakReference<TOwner>(owner);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TOwner Owner
        {
            get
            {
                TOwner r = null;
                _Owner?.TryGetTarget(out r);
                return r;
            }
        }

        object IOwnedCollection.Owner
            => Owner;

        protected override void ClearItems()
        {
            var ow = Owner;
            if (ow != null)
            {
                foreach (var s in this)
                {
                    (s as IHasParentInternal)?.SetParent(null);
                }
            }
            base.ClearItems();
        }

        protected override void InsertItem(int index, TItem item)
        {
            var ow = Owner;
            if (ow != null)
            {
                if (item.Parent != null)
                {
                    throw new ArgumentException();
                }
                (item as IHasParentInternal)?.SetParent(this);
            }

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            var ow = Owner;
            if (ow != null)
            {
                var item = this[index];
                (item as IHasParentInternal)?.SetParent(null);
            }
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, TItem item)
        {
            var ow = Owner;
            if (ow != null)
            {
                var old = this[index];
                if (old == item)
                {
                    return;
                }
                (old as IHasParentInternal)?.SetParent(null);
                (item as IHasParentInternal)?.SetParent(this);
            }
            base.SetItem(index, item);
        }
    }
}