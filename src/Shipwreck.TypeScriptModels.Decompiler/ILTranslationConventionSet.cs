using System;
using System.Collections.Generic;
using System.Linq;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public class ILTranslationConventionSet : ILTranslationConvention
    {
        private readonly ILTranslationConvention[] _Items;

        public ILTranslationConventionSet(params ILTranslationConvention[] items)
            : this(items, true)
        {
        }

        protected ILTranslationConventionSet(ILTranslationConvention[] items, bool copyItems)
        {
            _Items = copyItems ? items.ToArray() : items;
        }

        public override void ApplyTo(ILTranslator translator)
        {
            foreach (var item in _Items)
            {
                item.ApplyTo(translator);
            }
        }
    }
}