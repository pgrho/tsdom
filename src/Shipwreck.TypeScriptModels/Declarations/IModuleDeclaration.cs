using System.Collections;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public interface IModuleDeclaration
    {
        /// <summary>
        /// Gets of sets the identifier of the module declaration.
        /// </summary>
        string Name { get; set; }


        IList Members { get; }
    }
}