using System;
using System.Collections.ObjectModel;

namespace Shipwreck.TypeScriptModels
{
    public sealed class TypeScriptCollection<T> : Collection<T>
        where T : Statement
    {
        private readonly ITypeScriptObjectOwner _Owner;

        /// <summary>
        /// <see cref="TypeScriptCollection{T}" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public TypeScriptCollection()
        {
        }

        internal TypeScriptCollection(ITypeScriptObjectOwner owner)
        {
            _Owner = owner;
        }

        protected override void ClearItems()
        {
            if (_Owner != null)
            {
                foreach (var item in this)
                {
                    item.Owner = null;
                }
            }
            base.ClearItems();
        }

        protected override void InsertItem(int index, T item)
        {
            if (_Owner != null)
            {
                if (item.Owner != null)
                {
                    throw new InvalidOperationException();
                }
                item.Owner = _Owner;
            }
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            base.RemoveItem(index);

            if (_Owner != null)
            {
                item.Owner = null;
            }
        }

        protected override void SetItem(int index, T item)
        {
            var old = this[index];

            if (_Owner != null)
            {
                if (item == old)
                {
                    return;
                }
                if (item.Owner != null)
                {
                    throw new InvalidOperationException();
                }
                item.Owner = _Owner;
            }
            base.SetItem(index, item);

            if (_Owner != null)
            {
                old.Owner = null;
            }
        }
    }
}