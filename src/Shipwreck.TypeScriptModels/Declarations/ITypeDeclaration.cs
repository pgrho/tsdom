using System.Collections;
using System.Collections.ObjectModel;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public interface ITypeDeclaration : ITypeReference, IRootStatement, INamespaceMember, IModuleMember
    {
        /// <summary>
        /// Gets or sets the value indicating whether the type has a <c>declare</c> modifier.
        /// </summary>
        bool IsDeclare { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the type has a <c>export</c> modifier.
        /// </summary>
        bool IsExport { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the type.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Members" /> contains any element;
        /// </summary>
        bool HasMember { get; }

        /// <summary>
        /// Gets or sets the all members of the type.
        /// </summary>
        IList Members { get; }

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Decorators" /> contains any element;
        /// </summary>
        bool HasDecorator { get; }

        /// <summary>
        /// Gets or sets the all decorators of the type.
        /// </summary>
        Collection<Decorator> Decorators { get; }
    }
}