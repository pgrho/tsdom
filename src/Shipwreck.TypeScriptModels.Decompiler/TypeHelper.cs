using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using System;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public static class TypeHelper
    {
        public static bool IsEquivalentTo(this Type clrType, IType declaration)
            => false;

        public static bool IsEquivalentTo(this Type clrType, TypeDeclaration declaration)
            => false;
    }
}