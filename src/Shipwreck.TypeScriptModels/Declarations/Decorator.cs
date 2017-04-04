using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class Decorator : Syntax
    {
        [DefaultValue(null)]
        public string Name { get; set; }

        #region Parameters

        private Collection<Expression> _Parameters;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Parameters" /> contains any element;
        /// </summary>
        public bool HasParameter
            => _Parameters?.Count > 0;

        /// <summary>
        /// Gets or sets the all parameters of the decorator.
        /// </summary>
        public Collection<Expression> Parameters
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Parameters);
            }
            set
            {
                CollectionHelper.Set(ref _Parameters, value);
            }
        }

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="Parameters" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializeParameters()
            => HasParameter;

        /// <summary>
        /// Resets the value for <see cref="Parameters" /> of the decorator to the default value.
        /// </summary>
        public void ResetParameters()
            => _Parameters?.Clear();

        #endregion Parameters

    }
}