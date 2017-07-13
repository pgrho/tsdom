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
    public sealed class ClassDeclaration : TypeDeclaration<IClassMember>
    {
        public override bool? IsClass => true;

        /// <summary>
        /// Gets or sets the value indicating whether the class has a <c>default</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the class has a <c>abstract</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsAbstract { get; set; }

        [DefaultValue(false)]
        public ITypeReference BaseType { get; set; }

        #region Interfaces

        private Collection<ITypeReference> _Interfaces;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Interfaces" /> contains any element;
        /// </summary>
        public bool HasInterface
            => _Interfaces?.Count > 0;

        /// <summary>
        /// Gets or sets the all implemented interfaces of the class.
        /// </summary>
        public Collection<ITypeReference> Interfaces
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
        /// Determines a value indicating whether the value of <see cref="Interfaces" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializeInterfaces()
            => HasInterface;

        /// <summary>
        /// Resets the value for <see cref="Interfaces" /> of the class to the default value.
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

        public override void WriteTypeReference(TextWriter writer)
        {
            writer.Write(Name);
            writer.WriteTypeParameters(_TypeParameters);
        }

        /// <inheritdoc />
        public override void Accept<T>(IRootStatementVisitor<T> visitor)
            => visitor.VisitClassDeclaration(this);

        /// <inheritdoc />
        public override void Accept<T>(IModuleMemberVisitor<T> visitor)
            => visitor.VisitClassDeclaration(this);

        /// <inheritdoc />
        public override void Accept<T>(INamespaceMemberVisitor<T> visitor)
            => visitor.VisitClassDeclaration(this);
    }
}