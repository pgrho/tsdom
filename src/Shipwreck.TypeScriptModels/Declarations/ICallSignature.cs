using System.Collections.ObjectModel;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public interface ICallSignature
    {
        bool HasParameter { get; }
        bool HasTypeParameter { get; }
        Collection<Parameter> Parameters { get; set; }
        ITypeReference ReturnType { get; set; }
        Collection<TypeParameter> TypeParameters { get; set; }
    }
}