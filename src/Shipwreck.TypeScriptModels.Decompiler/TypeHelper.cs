using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using System;
using System.Linq;
using System.Reflection;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public static class TypeHelper
    {
        public static Type ResolveClrType(this AstType type)
            => type.Annotation<Type>() ?? type.Annotation<TypeReference>()?.ResolveClrType();

        public static Type ResolveClrType(this TypeReference type)
        {
            var git = type as GenericInstanceType;

            if (git != null)
            {
                var i = git.FullName.IndexOf('<');
                if (i >= 0)
                {
                    var dfn = git.FullName.Substring(0, i);
                    var dt = Type.GetType(dfn + "," + type.Scope.ToString(), false, false) ?? Type.GetType(dfn, false, false);

                    if (dt != null)
                    {
                        var ps = git.GenericArguments.Select(p => p.ResolveClrType()).ToArray();

                        if (Array.IndexOf(ps, null) < 0)
                        {
                            return dt.MakeGenericType(ps);
                        }
                    }

                    return null;
                }
            }

            // TODO: assembly nameに拡張子がついている場合？？？
            var an = new AssemblyName(type.Scope.ToString());

            var fn = type.FullName.Replace('/', '+');
            return Type.GetType(fn + "," + an, false, false) ?? Type.GetType(fn, false, false);
        }

        public static bool IsEquivalentTo(this Type clrType, TypeReference cecilType)
            => clrType == null ? cecilType == null : clrType.FullName == cecilType?.FullName; // TODO: generic test

        public static bool IsBaseTypeOf(this Type clrType, TypeReference cecilType)
        {
            var cct = Type.GetType(cecilType.FullName + "," + cecilType.Module.Name) ?? Type.GetType(cecilType.FullName);

            if (cct != null)
            {
                while (cct != null)
                {
                    if (clrType == cct)
                    {
                        return true;
                    }
                    cct = cct.BaseType;
                }

                return false;
            }
            else
            {
                var bt = cecilType;

                while (bt != null)
                {
                    if (clrType.IsEquivalentTo(bt))
                    {
                        return true;
                    }
                    if (bt.Namespace == nameof(System)
                        && bt.Name == nameof(Object))
                    {
                        return false;
                    }
                    bt = bt.Module.AssemblyResolver != null ? bt.Resolve().BaseType : null;
                }

                return false;
            }
        }
    }
}