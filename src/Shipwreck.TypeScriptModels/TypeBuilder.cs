using Shipwreck.TypeScriptModels.Declarations;

namespace Shipwreck.TypeScriptModels
{
    public static class TypeBuilder
    {
        public static ArrayType MakeArrayType(this ITypeReference type)
            => new ArrayType(type);

        public static UnionType UnionWith(this ITypeReference type, ITypeReference other)
        {
            var ut = new UnionType();
            ut.ElementTypes.Add(type);
            ut.ElementTypes.Add(other);

            return ut;
        }
    }
}