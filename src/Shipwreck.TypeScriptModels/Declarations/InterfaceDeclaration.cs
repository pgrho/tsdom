﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class InterfaceDeclaration : TypeDeclaration
    {
        #region TypeParameters

        private Collection<TypeParameter> _TypeParameters;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="TypeParameters" /> contains any element;
        /// </summary>
        public bool HasTypeParameter
            => _TypeParameters?.Count > 0;

        /// <summary>
        /// Gets or sets the all generic type parameter definitions of the interface.
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
        /// Resets the value for <see cref="TypeParameters" /> of the interface to the default value.
        /// </summary>
        public void ResetTypeParameters()
            => _TypeParameters?.Clear();

        #endregion TypeParameters

        #region BaseTypes

        private Collection<ITypeReference> _BaseTypes;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="BaseTypes" /> contains any element;
        /// </summary>
        public bool HasBaseType
            => _BaseTypes?.Count > 0;

        /// <summary>
        /// Gets or sets the all base interfaces of the interface.
        /// </summary>
        public Collection<ITypeReference> BaseTypes
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _BaseTypes);
            }
            set
            {
                CollectionHelper.Set(ref _BaseTypes, value);
            }
        }

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="BaseTypes" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializeBaseTypes()
            => HasBaseType;

        /// <summary>
        /// Resets the value for <see cref="BaseTypes" /> of the interface to the default value.
        /// </summary>
        public void ResetBaseTypes()
            => _BaseTypes?.Clear();

        #endregion BaseTypes


        #region Members

        private Collection<IInterfaceMember> _Members;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Members" /> contains any element;
        /// </summary>
        public bool HasMember
            => _Members?.Count > 0;

        /// <summary>
        /// Gets or sets the all members of the object.
        /// </summary>
        public Collection<IInterfaceMember> Members
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
        /// Resets the value for <see cref="Members" /> of the object to the default value.
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