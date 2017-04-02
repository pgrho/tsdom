using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels
{
    public abstract class Expression
    {
        [ThreadStatic]
        private static WeakReference<TextWriter> _CachedTextWriter;

        [ThreadStatic]
        private static WeakReference<TypeScriptWriter> _CachedTypeScriptWriter;

        public abstract void Accept<T>(IExpressionVisitor<T> visitor);

        public void WriteExpression(TextWriter writer)
        {
            TextWriter w = null;
            TypeScriptWriter visitor;
            _CachedTextWriter?.TryGetTarget(out w);

            if (w == null && _CachedTypeScriptWriter.TryGetTarget(out visitor))
            {
                Accept(visitor);
            }
            else
            {
                visitor = new TypeScriptWriter(writer);
                _CachedTextWriter = new WeakReference<TextWriter>(writer);
                _CachedTypeScriptWriter = new WeakReference<TypeScriptWriter>(visitor);

                Accept(visitor);
            }
        }
    }
}
