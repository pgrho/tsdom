using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Declarations;

namespace Shipwreck.TypeScriptModels
{
    internal static class TextWriterHelper
    {
        public static void WriteLiteral(this TextWriter writer, string value)
        {
            if (value == null)
            {
                writer.Write("null");
            }
            else
            {
                writer.Write('"');
                foreach (var c in value)
                {
                    // TODO: 仕様確認
                    switch (c)
                    {
                        case '\0':
                            writer.Write("\\0");
                            break;

                        case '\a':
                            writer.Write("\\a");
                            break;

                        case '\b':
                            writer.Write("\\b");
                            break;

                        case '\r':
                            writer.Write("\\c");
                            break;

                        case '\n':
                            writer.Write("\\n");
                            break;

                        case '\t':
                            writer.Write("\\t");
                            break;

                        case '"':
                            writer.Write("\\\"");
                            break;

                        default:
                            writer.Write(c);
                            break;
                    }
                }
                writer.Write('"');
            }
        }

        public static void WriteAccessibility(this TextWriter writer, AccessibilityModifier accessibility)
        {
            switch (accessibility)
            {
                case AccessibilityModifier.None:
                    break;

                case AccessibilityModifier.Public:
                    writer.Write("public ");
                    break;

                case AccessibilityModifier.Private:
                    writer.Write("private ");
                    break;

                case AccessibilityModifier.Protected:
                    writer.Write("protected ");
                    break;

                default:
                    throw new NotImplementedException();
            }
        }


        public static void WriteCallSignature(this TextWriter writer, ICallSignature signature)
            => writer.WriteCallSignature<object>(signature, null);

        internal static void WriteCallSignature<T>(this TextWriter writer, ICallSignature signature, IExpressionVisitor<T> visitor)
        {
            writer.WriteTypeParameters(signature.TypeParameters);
            if (signature.HasParameter)
            {
                writer.WriteParameters(signature.Parameters);
            }
            else
            {
                writer.Write("()");
            }
            if (signature.ReturnType != null)
            {
                writer.Write(": ");
                signature.ReturnType.WriteTypeReference(writer);
            }
        }

        public static void WriteTypeParameters(this TextWriter writer, Collection<TypeParameter> typeParameters)
        {
            if (typeParameters?.Count > 0)
            {
                writer.Write('<');

                for (var i = 0; i < typeParameters.Count; i++)
                {
                    if (i > 0)
                    {
                        writer.Write(", ");
                    }
                    var tp = typeParameters[i];
                    writer.Write(tp.Name);

                    if (tp.Constraint != null)
                    {
                        writer.Write(" extends ");
                        tp.Constraint.WriteTypeReference(writer);
                    }
                }

                writer.Write('>');
            }
        }
        public static void WriteTypeParameters(this TextWriter writer, Collection<ITypeReference> typeParameters)
        {
            if (typeParameters?.Count > 0)
            {
                writer.Write('<');

                for (var i = 0; i < typeParameters.Count; i++)
                {
                    if (i > 0)
                    {
                        writer.Write(", ");
                    }
                    var tp = typeParameters[i];
                    tp.WriteTypeReference(writer);
                }

                writer.Write('>');
            }
        }

        public static void WriteParameters(this TextWriter writer, Collection<Parameter> parameters)
            => writer.WriteParameters<object>(parameters, null);

        internal static void WriteParameters<T>(this TextWriter writer, Collection<Parameter> parameters, IExpressionVisitor<T> visitor = null)
        {
            writer.Write('(');

            if (parameters?.Count > 0)
            {
                for (var i = 0; i < parameters.Count; i++)
                {
                    if (i > 0)
                    {
                        writer.Write(", ");
                    }

                    var tp = parameters[i];
                    if (tp.IsRest)
                    {
                        writer.Write("...");
                    }
                    writer.WriteAccessibility(tp.Accessibility);
                    writer.Write(tp.ParameterName);
                    if (tp.IsOptional && tp.Initializer != null)
                    {
                        writer.Write('?');
                    }

                    if (tp.ParameterType != null)
                    {
                        writer.Write(": ");
                        tp.ParameterType.WriteTypeReference(writer);
                    }

                    if (tp.Initializer != null)
                    {
                        writer.Write(" = ");

                        if (visitor == null)
                        {
                            tp.Initializer.WriteExpression(writer);
                        }
                        else
                        {
                            tp.Initializer.Accept(visitor);
                        }
                    }
                }
            }
            writer.Write(')');
        }
        public static void WriteParameters<T>(this TextWriter writer, Collection<Expression> parameters, IExpressionVisitor<T> visitor)
        {
            writer.Write('(');

            if (parameters?.Count > 0)
            {
                for (var i = 0; i < parameters.Count; i++)
                {
                    if (i > 0)
                    {
                        writer.Write(", ");
                    }

                    parameters[i].Accept(visitor);
                }

            }
            writer.Write(')');
        }
    }
}
