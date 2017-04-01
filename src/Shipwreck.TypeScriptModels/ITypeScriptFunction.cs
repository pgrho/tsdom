using System.Collections.Generic;

namespace Shipwreck.TypeScriptModels
{
    public interface ITypeScriptFunction
    {
        string Name { get; set; }

        Documentation Documentation { get; set; }

        List<ParameterDeclaration> Parameters { get; set; }
        ITypeScriptType ReturnType { get; set; }
    }
}