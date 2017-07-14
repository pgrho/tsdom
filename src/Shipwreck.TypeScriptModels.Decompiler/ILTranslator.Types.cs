using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using D = Shipwreck.TypeScriptModels.Declarations;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    partial class ILTranslator
    {
        protected virtual D.IModuleDeclaration ResolveModule(ILTransformationContext data, string fullName)
        {
            var ms = Statements;
            var m = ms.OfType<D.IModuleDeclaration>().FirstOrDefault(e => e.Name == fullName);
            if (m == null)
            {
                m = new D.NamespaceDeclaration()
                {
                    Name = fullName
                };
                ms.Add(m);
            }

            return m;
        }

        #region 名前空間レベル

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, ILTransformationContext data)
            => OnVisiting(data, namespaceDeclaration, VisitingNamespaceDeclaration)
            ?? OnVisited(data, namespaceDeclaration, VisitedNamespaceDeclaration, TranslateNamespaceDeclaration(namespaceDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, ILTransformationContext data)
        {
            var ns = ResolveModule(data, namespaceDeclaration.FullName);
            foreach (var c in namespaceDeclaration.Children)
            {
                if (c is MemberType)
                {
                    continue;
                }
                if (c is NamespaceDeclaration)
                {
                    foreach (var cr in c.AcceptVisitor(this, data))
                    {
                        yield return cr;
                    }
                }
                else
                {
                    foreach (var cr in c.AcceptVisitor(this, data))
                    {
                        ns.Members.Add(cr);
                    }
                }
            }

            yield return (Syntax)ns;
        }

        #endregion 名前空間レベル

        #region 型レベル

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitTypeDeclaration(TypeDeclaration typeDeclaration, ILTransformationContext data)
            => OnVisiting(data, typeDeclaration, VisitingTypeDeclaration)
                ?? OnVisited(data, typeDeclaration, VisitedTypeDeclaration, TranslateTypeDeclaration(typeDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateTypeDeclaration(TypeDeclaration typeDeclaration, ILTransformationContext data)
        {
            D.ITypeDeclaration td;
            switch (typeDeclaration.ClassType)
            {
                case ClassType.Class:
                    {
                        var cd = new D.ClassDeclaration()
                        {
                            IsAbstract = typeDeclaration.HasModifier(Modifiers.Abstract)
                        };
                        td = cd;

                        foreach (var p in GetTypeParameters(typeDeclaration.TypeParameters, typeDeclaration.Constraints))
                        {
                            cd.TypeParameters.Add(p);
                        }
                    }
                    break;

                case ClassType.Struct:
                    {
                        var cd = new D.ClassDeclaration();

                        td = cd;

                        foreach (var p in GetTypeParameters(typeDeclaration.TypeParameters, typeDeclaration.Constraints))
                        {
                            cd.TypeParameters.Add(p);
                        }
                    }
                    break;

                case ClassType.Interface:
                    var id = new D.InterfaceDeclaration();
                    td = id;

                    foreach (var p in GetTypeParameters(typeDeclaration.TypeParameters, typeDeclaration.Constraints))
                    {
                        id.TypeParameters.Add(p);
                    }

                    break;

                case ClassType.Enum:
                    td = new D.EnumDeclaration();
                    break;

                default:
                    throw GetNotImplementedException();
            }

            var decType = typeDeclaration.Parent as TypeDeclaration;
            td.Name = typeDeclaration.Name;
            td.IsExport = typeDeclaration.HasModifier(Modifiers.Public);

            while (decType != null)
            {
                td.Name = decType.Name + "$" + td.Name;
                td.IsExport &= decType.HasModifier(Modifiers.Public);
                decType = decType.Parent as TypeDeclaration;
            }

            foreach (var bt in typeDeclaration.BaseTypes)
            {
                var dt = ResolveType(typeDeclaration, bt);

                var cd = td as D.ClassDeclaration;
                if (cd != null && cd.BaseType == null && dt.IsClass == true)
                {
                    cd.BaseType = dt;
                }
                else if (cd != null)
                {
                    cd.Interfaces.Add(dt);
                }
                else
                {
                    ((D.InterfaceDeclaration)td).BaseTypes.Add(dt);
                }
            }

            foreach (var c in typeDeclaration.Attributes)
            {
                foreach (var cr in c.AcceptVisitor(this, data))
                {
                    td.Decorators.Add((D.Decorator)cr);
                }
            }

            foreach (var c in typeDeclaration.Members)
            {
                var ctd = c as TypeDeclaration;
                if (ctd != null)
                {
                    foreach (var cr in c.AcceptVisitor(this, data))
                    {
                        yield return cr;
                    }
                    continue;
                }
                foreach (var cr in c.AcceptVisitor(this, data))
                {
                    td.Members.Add(cr);
                }
            }

            yield return (Syntax)td;
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitAttributeSection(AttributeSection attributeSection, ILTransformationContext data)
            => OnVisiting(data, attributeSection, VisitingAttributeSection)
            ?? OnVisited(data, attributeSection, VisitedAttributeSection, TranslateAttributeSection(attributeSection, data));

        protected virtual IEnumerable<Syntax> TranslateAttributeSection(AttributeSection attributeSection, ILTransformationContext data)
        {
            foreach (var c in attributeSection.Children)
            {
                foreach (var cr in c.AcceptVisitor(this, data))
                {
                    yield return cr;
                }
            }
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitAttribute(ICSharpCode.NRefactory.CSharp.Attribute attribute, ILTransformationContext data)
        {
            var d = new D.Decorator();
            d.Name = GetTypeName(attribute.Type);

            foreach (var c in attribute.Children)
            {
                if (c is SimpleType)
                {
                    continue;
                }
                else
                {
                    foreach (var cr in c.AcceptVisitor(this, data))
                    {
                        d.Parameters.Add((Expression)cr);
                    }
                }
            }

            yield return d;
        }

        private Collection<D.Decorator> GetDecorators(IEnumerable<AttributeSection> section, ILTransformationContext data)
        {
            Collection<D.Decorator> c = null;
            foreach (var s in section)
            {
                foreach (var a in s.Attributes)
                {
                    foreach (D.Decorator d in a.AcceptVisitor(this, data))
                    {
                        (c ?? (c = new Collection<D.Decorator>())).Add(d);
                    }
                }
            }
            return c;
        }

        private IEnumerable<D.TypeParameter> GetTypeParameters(AstNodeCollection<TypeParameterDeclaration> typeParameters, AstNodeCollection<Constraint> constraints)
        {
            foreach (var p in typeParameters)
            {
                var gp = new D.TypeParameter();
                gp.Name = p.Name;

                ITypeReference ct = null;
                foreach (var c in constraints)
                {
                    if (c.TypeParameter.Identifier == p.Name)
                    {
                        foreach (var bt in c.BaseTypes)
                        {
                            var pt = bt as PrimitiveType;
                            if (pt?.Keyword == "struct"
                                || pt?.Keyword == "class"
                                || pt?.Keyword == "new")
                            {
                                continue;
                            }

                            var nt = ResolveType(c, bt);

                            if (ct == null)
                            {
                                ct = nt;
                            }
                            else if (ct is D.UnionType)
                            {
                                ((D.UnionType)ct).ElementTypes.Add(nt);
                            }
                            else
                            {
                                var ut = new D.UnionType();
                                ut.ElementTypes.Add(ct);
                                ut.ElementTypes.Add(nt);
                                ct = ut;
                            }
                        }
                    }
                }
                gp.Constraint = ct;

                yield return gp;
            }
        }

        #endregion 型レベル

        #region 対象外

        #region usingディレクティブ

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitUsingAliasDeclaration(UsingAliasDeclaration usingAliasDeclaration, ILTransformationContext data)
            => Enumerable.Empty<Syntax>();

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitUsingDeclaration(UsingDeclaration usingDeclaration, ILTransformationContext data)
            => Enumerable.Empty<Syntax>();

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitPreProcessorDirective(PreProcessorDirective preProcessorDirective, ILTransformationContext data)
            => Enumerable.Empty<Syntax>();

        #endregion usingディレクティブ

        #endregion 対象外
    }
}