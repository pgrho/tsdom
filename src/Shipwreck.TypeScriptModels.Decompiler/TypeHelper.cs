using Mono.Cecil;
using System;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public static class TypeHelper
    {
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