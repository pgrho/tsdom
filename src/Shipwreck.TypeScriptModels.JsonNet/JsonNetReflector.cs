using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Shipwreck.TypeScriptModels.Declarations;
using Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.JsonNet
{
    public class JsonNetReflector
    {
        // TODO: support generic types
        // TODO: support module or namespace

        public IEnumerable<object> GetDeclarations(IEnumerable<Type> clrTypes)
        {
            var processingTypes = clrTypes.Distinct().ToList();
            var stringUnionEnums = new Dictionary<Type, TypeAliasDeclaration>();
            var decs = new Dictionary<Type, IRootStatement>();

            for (var i = 0; i < processingTypes.Count; i++)
            {
                var t = processingTypes[i];
                var td = GetTypeDeclaration(t, processingTypes, stringUnionEnums);
                if (td != null)
                {
                    decs[t] = td;
                }
            }

            foreach (var kv in stringUnionEnums)
            {
                if (decs.ContainsKey(kv.Key))
                {
                    kv.Value.Name += "$AsString";
                }
            }

            return stringUnionEnums.Values.Cast<object>().Concat(decs.Values);
        }

        protected virtual string GetTypeName(Type type) => type.Name;

        protected virtual bool ShouldDeclareType(Type type) => type != typeof(object);

        private ITypeDeclaration GetTypeDeclaration(Type type, List<Type> processingTypes, Dictionary<Type, TypeAliasDeclaration> stringUnionEnums)
        {
            var ti = type.GetTypeInfo();
            var isDataContract = ti.GetCustomAttribute<DataContractAttribute>() != null;

            if (ti.IsEnum)
            {
                var ed = new EnumDeclaration();
                ed.IsDeclare = true;
                ed.Name = GetTypeName(type);

                foreach (var f in ti.DeclaredFields)
                {
                    if (isDataContract && f.GetCustomAttribute<EnumMemberAttribute>() == null)
                    {
                        continue;
                    }

                    var ef = new FieldDeclaration();
                    ef.FieldName = f.Name;
                    ef.Initializer = new NumberExpression(((IConvertible)f.GetValue(null)).ToDouble(null));

                    ed.Members.Add(ef);
                }

                return ed;
            }
            else
            {
                var id = new InterfaceDeclaration();
                id.IsDeclare = true;
                id.Name = GetTypeName(type);

                if (ti.BaseType != null
                    && ti.BaseType != typeof(object))
                {
                    if (ShouldDeclareType(ti.BaseType))
                    {
                        id.BaseTypes.Add(GetTypeReference(ti.BaseType, processingTypes, stringUnionEnums));
                    }
                }

                foreach (var it in ti.ImplementedInterfaces)
                {
                    if (ShouldDeclareType(it))
                    {
                        id.BaseTypes.Add(GetTypeReference(it, processingTypes, stringUnionEnums));
                    }
                }

                foreach (var f in ti.DeclaredFields.Where(f => !f.IsStatic))
                {
                    AddMember(type, isDataContract, id, f, processingTypes, stringUnionEnums);
                }
                foreach (var p in ti.DeclaredProperties.Where(f => f.GetMethod?.IsStatic == false))
                {
                    AddMember(type, isDataContract, id, p, processingTypes, stringUnionEnums);
                }

                return id;
            }
        }

        private void AddMember(Type type, bool isDataContract, InterfaceDeclaration declaration, MemberInfo member, List<Type> processingTypes, Dictionary<Type, TypeAliasDeclaration> stringUnionEnums)
        {
            if (member.GetCustomAttribute<JsonIgnoreAttribute>() != null
                && member.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
            {
                return;
            }

            var dm = member.GetCustomAttribute<DataMemberAttribute>();

            if (isDataContract && dm == null)
            {
                return;
            }

            var jp = member.GetCustomAttribute<JsonPropertyAttribute>();

            var fd = new FieldDeclaration();

            fd.FieldName = jp.PropertyName ?? dm?.Name ?? member.Name;

            var ft = member is FieldInfo fld ? fld.FieldType : ((PropertyInfo)member).PropertyType;

            if (ft.IsConstructedGenericType && ft.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                ft = ft.GenericTypeArguments[0];
                fd.IsOptional = true;
            }
            else
            {
                fd.IsOptional = !ft.GetTypeInfo().IsValueType
                                    && ((jp != null && jp.Required != Required.Default)
                                        || member.GetCustomAttribute<RequiredAttribute>() != null);
            }

            if (ft.GetTypeInfo().IsEnum
                && member.GetCustomAttribute<JsonConverterAttribute>()?.ConverterType == typeof(StringEnumConverter))
            {
                if (!stringUnionEnums.TryGetValue(ft, out var ta))
                {
                    ta = new TypeAliasDeclaration();
                    ta.Name = GetTypeName(ft);

                    var ut = new UnionType();
                    foreach (var ftf in ft.GetTypeInfo().DeclaredFields)
                    {
                        var em = ftf.GetCustomAttribute<EnumMemberAttribute>();

                        if (isDataContract && em == null)
                        {
                            continue;
                        }

                        ut.ElementTypes.Add(new StringLiteralType() { Value = em?.Value ?? ftf.Name });
                    }

                    ta.Type = ut;

                    stringUnionEnums[ft] = ta;
                }
                fd.FieldType = ta;
            }
            else
            {
                fd.FieldType = GetTypeReference(ft, processingTypes, stringUnionEnums);
            }

            declaration.Members.Add(fd);
        }

        private ITypeReference GetTypeReference(Type type, List<Type> processingTypes, Dictionary<Type, TypeAliasDeclaration> stringUnionEnums)
        {
            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return GetTypeReference(type.GenericTypeArguments[0], processingTypes, stringUnionEnums);
            }
            if (type.Namespace == nameof(System))
            {
                switch (type.Name)
                {
                    case nameof(Object):
                        return new NamedTypeReference() { Name = "any" };

                    case nameof(Boolean):
                        return new NamedTypeReference() { Name = "boolean" };

                    case nameof(Byte):
                    case nameof(SByte):
                    case nameof(Int16):
                    case nameof(UInt16):
                    case nameof(Int32):
                    case nameof(UInt32):
                    case nameof(Int64):
                    case nameof(UInt64):
                    case nameof(Single):
                    case nameof(Double):
                    case nameof(Decimal):
                        return new NamedTypeReference() { Name = "number" };

                    case nameof(DateTime):
                    case nameof(TimeSpan):
                    case nameof(DateTimeOffset):
                    case nameof(String):
                    case nameof(Guid):
                        return new NamedTypeReference() { Name = "string" };

                    case nameof(Array):
                        return new NamedTypeReference() { Name = "Array" };
                }
            }

            var typeInfo = type.GetTypeInfo();
            var elemType = typeInfo.ImplementedInterfaces
                                    .Select(it => it.IsConstructedGenericType
                                                && it.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                                    ? it.GenericTypeArguments[0]
                                                    : null)
                                    .OfType<Type>()
                                    .FirstOrDefault();

            if (elemType != null)
            {
                return new ArrayType(GetTypeReference(elemType, processingTypes, stringUnionEnums));
            }

            if (!processingTypes.Contains(type))
            {
                processingTypes.Add(type);
            }
            return new NamedTypeReference() { Name = GetTypeName(type) };
        }
    }
}