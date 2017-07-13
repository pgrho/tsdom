using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public abstract class FunctionTypeBase : ITypeReference
    {
        public bool? IsClass => true;

        public bool? IsInterface => false;

        public bool? IsEnum => false;

        #region TypeParameters

        private Collection<TypeParameter> _TypeParameters;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="TypeParameters" /> contains any element;
        /// </summary>
        public bool HasTypeParameter
            => _TypeParameters?.Count > 0;

        /// <summary>
        /// Gets or sets the all generic type parameter definitions of the function.
        /// </summary>
        public Collection<TypeParameter> TypeParameters
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _TypeParameters);
            }
            set
            {
                CollectionHelper.Set(ref _TypeParameters, value);
            }
        }

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="TypeParameters" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializeTypeParameters()
            => HasTypeParameter;

        /// <summary>
        /// Resets the value for <see cref="TypeParameters" /> of the function to the default value.
        /// </summary>
        public void ResetTypeParameters()
            => _TypeParameters?.Clear();

        #endregion TypeParameters

        #region Parameters

        private Collection<Parameter> _Parameters;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Parameters" /> contains any element;
        /// </summary>
        public bool HasParameter
            => _Parameters?.Count > 0;

        /// <summary>
        /// Gets or sets the all parameter definitions of the function.
        /// </summary>
        public Collection<Parameter> Parameters
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
        /// Resets the value for <see cref="Parameters" /> of the function to the default value.
        /// </summary>
        public void ResetParameters()
            => _Parameters?.Clear();

        #endregion Parameters

        /// <summary>
        /// Gets or sets the type of the value that the function returns.
        /// </summary>
        [DefaultValue(null)]
        public ITypeReference ReturnType { get; set; }

        public void WriteTypeReference(TextWriter writer)
        {
            WritePrefix(writer);
            writer.WriteTypeParameters(_TypeParameters);
            writer.WriteParameters(_Parameters);

            writer.Write(" => ");
            (ReturnType ?? PredefinedType.Any).WriteTypeReference(writer);
        }

        internal abstract void WritePrefix(TextWriter writer);
    }
}