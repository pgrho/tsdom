using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public abstract class TypeDeclaration<TMember> : Syntax, ITypeReference, ITypeDeclaration
    {
        /// <summary>
        /// Gets or sets the value indicating whether the type has a <c>declare</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDeclare { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the type has a <c>export</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsExport { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the type.
        /// </summary>
        [DefaultValue(null)]
        public string Name { get; set; }

        #region Members

        private Collection<TMember> _Members;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Members" /> contains any element;
        /// </summary>
        public bool HasMember
            => _Members?.Count > 0;

        /// <summary>
        /// Gets or sets the all members of the type.
        /// </summary>
        public Collection<TMember> Members
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

        IList ITypeDeclaration.Members => Members;

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="Members" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializMembers()
            => HasMember;

        /// <summary>
        /// Resets the value for <see cref="Members" /> of the type to the default value.
        /// </summary>
        public void ResetMembers()
            => _Members?.Clear();

        #endregion Members

        #region Decorators

        private Collection<Decorator> _Decorators;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Decorators" /> contains any element;
        /// </summary>
        public bool HasDecorator
            => _Decorators?.Count > 0;

        /// <summary>
        /// Gets or sets the all decorators of the type.
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
        /// Resets the value for <see cref="Decorators" /> of the type to the default value.
        /// </summary>
        public void ResetDecorators()
            => _Decorators?.Clear();

        #endregion Decorators

        public virtual void WriteTypeReference(TextWriter writer)
            => writer.Write(Name);

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="visitor">The visitor to visit this node with.</param>
        public abstract void Accept<T>(IRootStatementVisitor<T> visitor);

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="visitor">The visitor to visit this node with.</param>
        public abstract void Accept<T>(IModuleMemberVisitor<T> visitor);

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="visitor">The visitor to visit this node with.</param>
        public abstract void Accept<T>(INamespaceMemberVisitor<T> visitor);
    }
}