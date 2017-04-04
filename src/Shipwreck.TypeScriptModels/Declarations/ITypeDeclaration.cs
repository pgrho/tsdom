using System.Collections;
using System.Collections.ObjectModel;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public interface ITypeDeclaration : ITypeReference
    {
        bool IsDeclare { get; set; }
        bool IsExport { get; set; }
        string Name { get; set; }

        bool HasMember { get; }

        IList Members { get; }

        bool HasDecorator { get; }

        /// <summary>
        /// Gets or sets the all decorators of the type.
        /// </summary>
        Collection<Decorator> Decorators { get; }
    }
}