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
    public sealed class ClassDeclaration : TypeDeclaration
    {
        /// <summary>
        /// Gets or sets the value indicating whether the class has a <c>default</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDefault { get; set; }

        [DefaultValue(false)]
        public ITypeReference BaseType { get; set; }

        #region Interfaces

        private Collection<ITypeReference> _Interfaces;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Interface" /> contains any element;
        /// </summary>
        public bool HasInterface
            => _Interfaces?.Count > 0;

        /// <summary>
        /// Gets or sets the all implemented interfaces of the interface.
        /// </summary>
        public Collection<ITypeReference> Interface
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Interfaces);
            }
            set
            {
                CollectionHelper.Set(ref _Interfaces, value);
            }
        }

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="Interface" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializeInterfaces()
            => HasInterface;

        /// <summary>
        /// Resets the value for <see cref="Interface" /> of the interface to the default value.
        /// </summary>
        public void ResetInterfaces()
            => _Interfaces?.Clear();

        #endregion Interfaces

        #region TypeParameters

        private Collection<TypeParameter> _TypeParameters;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="TypeParameters" /> contains any element;
        /// </summary>
        public bool HasTypeParameter
            => _TypeParameters?.Count > 0;

        /// <summary>
        /// Gets or sets the all generic type parameter definitions of the class.
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
        /// Resets the value for <see cref="TypeParameters" /> of the class to the default value.
        /// </summary>
        public void ResetTypeParameters()
            => _TypeParameters?.Clear();

        #endregion TypeParameters

        #region Members

        private Collection<IClassMember> _Members;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Members" /> contains any element;
        /// </summary>
        public bool HasMember
            => _Members?.Count > 0;

        /// <summary>
        /// Gets or sets the all members of the class.
        /// </summary>
        public Collection<IClassMember> Members
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Members);
            }
            set
            {
                CollectionHelper.Set(ref _Members, value);
            }
        }

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="Members" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializMembers()
            => HasMember;

        /// <summary>
        /// Resets the value for <see cref="Members" /> of the class to the default value.
        /// </summary>
        public void ResetMembers()
            => _Members?.Clear();

        #endregion Members

        public override void WriteTypeReference(TextWriter writer)
        {
            writer.Write(Name);
            writer.WriteTypeParameters(_TypeParameters);
        }
    }
}