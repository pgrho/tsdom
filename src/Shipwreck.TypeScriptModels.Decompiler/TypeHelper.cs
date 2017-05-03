using Mono.Cecil;
using System;
using System.Linq;
using System.Reflection;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public static class TypeHelper
    {
        public static bool IsEquivalentTo(this Type clrType, TypeReference cecilType)
            => clrType == null ? cecilType == null : clrType.FullName == cecilType?.FullName; // TODO: generic test
    }

    public static class MethodHelper
    {
        public static bool IsEquivalentTo(this MethodInfo clrMethod, MethodReference cecilMethod)
        {
            if (clrMethod == null)
            {
                return cecilMethod == null;
            }

            if (clrMethod?.Name != cecilMethod?.Name
                || (clrMethod.IsGenericMethod ? clrMethod.GetGenericArguments().Length : 0) != (cecilMethod.HasGenericParameters ? cecilMethod.GenericParameters.Count : 0))
            {
                return false;
            }
            if (!clrMethod.DeclaringType.IsEquivalentTo(cecilMethod.DeclaringType))
            {
                if (clrMethod.IsVirtual)
                {
                    var bt = cecilMethod.DeclaringType.Resolve().BaseType?.Resolve();
                    while (bt != null)
                    {
                        if (clrMethod.DeclaringType.IsEquivalentTo(bt))
                        {
                            return bt.Methods.Any(m => m.Name == clrMethod.Name && HaveEquivalentParameters(clrMethod, m));
                        }

                        bt = bt.BaseType?.Resolve();
                    }
                }
                //var dec = cecilMethod as MethodDefinition;
                //if (dec != null)
                //{
                //    dec.Attributes 
                //}

                return false;
            }

            return HaveEquivalentParameters(clrMethod, cecilMethod);
        }

        private static bool HaveEquivalentParameters(MethodInfo clrMethod, MethodReference cecilMethod)
        {
            var cp = clrMethod.GetParameters();
            if (cp.Length != cecilMethod.Parameters.Count)
            {
                return false;
            }
            for (var i = 0; i < cp.Length; i++)
            {
                if (!cp[i].ParameterType.IsEquivalentTo(cecilMethod.Parameters[i].ParameterType))
                {
                    return false;
                }
            }
            // TODO: operator
            return true;
        }
    }
}