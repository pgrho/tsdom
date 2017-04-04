using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class Parameter
    {
        public AccessibilityModifier Accessibility { get; set; }
        public string ParameterName { get; set; }
        public ITypeReference ParameterType { get; set; }

        /// <summary>
        /// Gets or sets the expression to be assigned to the parameter if not specified.
        /// </summary>
        [DefaultValue(null)]
        public Expression Initializer { get; set; }

        public bool IsOptional { get; set; }
        public bool IsRest { get; set; }


        #region Decorators

        private Collection<Decorator> _Decorators;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Decorators" /> contains any element;
        /// </summary>
        public bool HasDecorator
            => _Decorators?.Count > 0;

        /// <summary>
        /// Gets or sets the all decorators of the parameter.
        /// </summary>
        public Collection<Decorator> Decorators
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Decorators);
            }
            set
            {
                CollectionHelper.Set(ref _Decorators, value);
            }
        }

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="Decorators" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializeDecorators()
            => HasDecorator;

        /// <summary>
        /// Resets the value for <see cref="Decorators" /> of the parameter to the default value.
        /// </summary>
        public void ResetDecorators()
            => _Decorators?.Clear();

        #endregion Decorators

    }
}
