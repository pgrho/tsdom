using System;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    internal static class ExceptionHelper
    {
        public static NotSupportedException CannotTranslateAst(string typeName)
            => new NotSupportedException($"Cannot translate {typeName}");
    }
}