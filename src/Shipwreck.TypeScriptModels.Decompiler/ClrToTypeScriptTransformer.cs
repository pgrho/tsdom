using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using D = Shipwreck.TypeScriptModels.Declarations;
using E = Shipwreck.TypeScriptModels.Expressions;
using S = Shipwreck.TypeScriptModels.Statements;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public class ClrToTypeScriptTransformer : IAstVisitor<string, IEnumerable<Syntax>>
    {
        private List<D.IRootStatement> _Statements;

        public List<D.IRootStatement> Statements
            => _Statements ?? (_Statements = new List<D.IRootStatement>());

        public IEnumerable<Syntax> Transform(Type clrType)
        {
            var ad = AssemblyDefinition.ReadAssembly(clrType.Assembly.Location);
            var b = new AstBuilder(new DecompilerContext(ad.MainModule)
            {
                Settings = new DecompilerSettings()
                {
                    AsyncAwait = true,
                    AutomaticProperties = true,
                    ExpressionTrees = true,
                    ForEachStatement = true,
                    LockStatement = true,
                    MakeAssignmentExpressions = true,
                    QueryExpressions = false,
                    UsingDeclarations = false,
                    YieldReturn = true
                }
            });
            b.DecompileMethodBodies = true;

            b.AddType(ad.MainModule.GetType(clrType.FullName));
            b.RunTransformations();

            return b.SyntaxTree.AcceptVisitor(this, "temp").ToArray();
        }

        protected virtual D.IModuleDeclaration ResolveModule(string data, string fullName)
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

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, string data)
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

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitTypeDeclaration(TypeDeclaration typeDeclaration, string data)
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
                    throw new NotImplementedException();
            }
            td.Name = typeDeclaration.Name;
            td.IsExport = typeDeclaration.HasModifier(Modifiers.Public);

            foreach (var c in typeDeclaration.Children)
            {
                if (c is CSharpModifierToken)
                {
                    continue;
                }
                else if (c is Identifier)
                {
                    continue;
                }
                else if (c is AttributeSection)
                {
                    foreach (var cr in c.AcceptVisitor(this, data))
                    {
                        td.Decorators.Add((D.Decorator)cr);
                    }
                }
                else
                {
                    foreach (var cr in c.AcceptVisitor(this, data))
                    {
                        td.Members.Add(cr);
                    }
                }
            }

            yield return (Syntax)td;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitAttribute(ICSharpCode.NRefactory.CSharp.Attribute attribute, string data)
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

        private Collection<D.Decorator> GetDecorators(IEnumerable<AttributeSection> section, string data)
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

                            var nt = GetTypeReference(bt);

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

        #region メンバーレベル

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitFieldDeclaration(FieldDeclaration fieldDeclaration, string data)
        {
            var bfd = new D.FieldDeclaration();
            bfd.Accessibility = GetAccessibility(fieldDeclaration.Modifiers);
            bfd.IsStatic = fieldDeclaration.HasModifier(Modifiers.Static);
            bfd.FieldName = fieldDeclaration.Name;
            bfd.FieldType = GetTypeReference(fieldDeclaration.ReturnType);

            yield return bfd;

            if (fieldDeclaration.Variables.Any())
            {
                foreach (var v in fieldDeclaration.Variables)
                {
                    if (v.Name == bfd.FieldName)
                    {
                        if (v.Initializer?.IsNull != false)
                        {
                            continue;
                        }
                        // TODO: initializer
                    }
                    else
                    {
                        var fd = new D.FieldDeclaration();
                        fd.Accessibility = GetAccessibility(fieldDeclaration.Modifiers);
                        fd.IsStatic = fieldDeclaration.HasModifier(Modifiers.Static);
                        fd.FieldName = v.Name;
                        fd.FieldType = GetTypeReference(fieldDeclaration.ReturnType);

                        // TODO: initializer

                        yield return fd;
                    }
                }
            }
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, string data)
        {
            var cd = new D.ConstructorDeclaration();

            // TODO: Accessiblity

            foreach (var p in GetParameters(data, constructorDeclaration.Parameters))
            {
                cd.Parameters.Add(p);
            }

            if (constructorDeclaration.Body.IsNull)
            {
                cd.Statements = GetStatements(data, constructorDeclaration.Body);
            }

            yield return cd;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitMethodDeclaration(MethodDeclaration methodDeclaration, string data)
        {
            // TODO: オーバーロード

            var md = new D.MethodDeclaration();
            md.Decorators = GetDecorators(methodDeclaration.Attributes, data);
            md.Accessibility = GetAccessibility(methodDeclaration.Modifiers);
            md.IsStatic = methodDeclaration.HasModifier(Modifiers.Static);
            md.IsAbstract = methodDeclaration.HasModifier(Modifiers.Abstract);

            md.MethodName = methodDeclaration.Name;

            foreach (var p in GetTypeParameters(methodDeclaration.TypeParameters, methodDeclaration.Constraints))
            {
                md.TypeParameters.Add(p);
            }

            foreach (var p in GetParameters(data, methodDeclaration.Parameters))
            {
                md.Parameters.Add(p);
            }

            md.ReturnType = GetTypeReference(methodDeclaration.ReturnType);

            if (methodDeclaration.Body.IsNull)
            {
                md.Statements = GetStatements(data, methodDeclaration.Body);
            }

            yield return md;
        }

        private IEnumerable<D.Parameter> GetParameters(string data, AstNodeCollection<ParameterDeclaration> parameters)
        {
            foreach (var p in parameters)
            {
                var dp = new D.Parameter()
                {
                    ParameterName = p.Name,
                };
                bool op;
                dp.ParameterType = GetTypeReference(p.Type, out op);

                dp.Decorators = GetDecorators(p.Attributes, data);

                if (p.DefaultExpression?.IsNull == false)
                {
                    dp.Initializer = (Expression)p.DefaultExpression.AcceptVisitor(this, data).LastOrDefault();
                }
                dp.IsOptional = op || dp.Initializer != null;
                yield return dp;
            }
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, string data)
        {
            var acc = GetAccessibility(propertyDeclaration.Modifiers);
            var tr = GetTypeReference(propertyDeclaration.ReturnType);

            // TODO: デコレーター

            // TODO: virtualの場合メソッドを生成する
            //if (propertyDeclaration.HasModifier(Modifiers.Abstract) || propertyDeclaration.HasModifier(Modifiers.Virtual))
            //{
            //}
            //else
            if (propertyDeclaration.Getter.IsNull
                    || propertyDeclaration.Getter.HasChildren
                    || propertyDeclaration.Setter.IsNull
                    || propertyDeclaration.Setter.HasChildren)
            {
                if (propertyDeclaration.Getter.Body.IsNull
                    && propertyDeclaration.Setter.Body.IsNull)
                {
                    var pd = new D.FieldDeclaration();
                    pd.Accessibility = D.AccessibilityModifier.Private;
                    pd.FieldName = "__" + propertyDeclaration.Name;
                    pd.FieldType = tr;

                    yield return pd;
                }

                if (!propertyDeclaration.Getter.IsNull)
                {
                    var getter = new D.GetAccessorDeclaration();
                    getter.Accessibility = propertyDeclaration.Getter.Modifiers != Modifiers.None ? GetAccessibility(propertyDeclaration.Getter.Modifiers) : acc;
                    getter.PropertyName = propertyDeclaration.Name;
                    getter.PropertyType = tr;

                    if (propertyDeclaration.Getter.Body.IsNull)
                    {
                        getter.Statements.Add(new S.ReturnStatement()
                        {
                            Value = new E.PropertyExpression()
                            {
                                Object = new E.ThisExpression(),
                                Property = "__" + propertyDeclaration.Name
                            }
                        });
                    }
                    else
                    {
                        getter.Statements = GetStatements(data, propertyDeclaration.Getter.Body);
                    }

                    yield return getter;
                }

                if (!propertyDeclaration.Setter.IsNull)
                {
                    var setter = new D.SetAccessorDeclaration();
                    setter.Accessibility = propertyDeclaration.Setter.Modifiers != Modifiers.None ? GetAccessibility(propertyDeclaration.Setter.Modifiers) : acc;
                    setter.PropertyName = propertyDeclaration.Name;
                    setter.PropertyType = tr;
                    setter.ParameterName = "value";

                    if (propertyDeclaration.Setter.Body.IsNull)
                    {
                        setter.Statements.Add(new S.ExpressionStatement()
                        {
                            Expression = new E.AssignmentExpression()
                            {
                                Target = new E.PropertyExpression()
                                {
                                    Object = new E.ThisExpression(),
                                    Property = "__" + propertyDeclaration.Name
                                },
                                Value = new E.IdentifierExpression()
                                {
                                    Name = "value"
                                }
                            }
                        });
                    }
                    else
                    {
                        setter.Statements = GetStatements(data, propertyDeclaration.Setter.Body);
                    }

                    yield return setter;
                }
            }
            else
            {
                var pd = new D.FieldDeclaration();
                pd.Accessibility = acc;
                pd.FieldName = propertyDeclaration.Name;
                pd.FieldType = tr;

                yield return pd;
            }
        }

        #endregion メンバーレベル

        #region ステートメントレベル

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitReturnStatement(ReturnStatement returnStatement, string data)
        {
            if (returnStatement.Expression?.IsNull == false)
            {
                yield return new S.ReturnStatement()
                {
                    Value = GetExpression(returnStatement.Expression, data)
                };
            }
            else
            {
                yield return new S.ReturnStatement();
            }
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitIfElseStatement(IfElseStatement ifElseStatement, string data)
        {
            throw new NotImplementedException();
        }

        #endregion ステートメントレベル

        #region 式レベル

        #region キーワード

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, string data)
        {
            yield return new E.ThisExpression();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, string data)
        {
            yield return new E.SuperExpression();
        }

        #endregion キーワード



        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitInvocationExpression(InvocationExpression invocationExpression, string data)
        {
            var inv = new E.CallExpression();

            var mre = invocationExpression.Target as MemberReferenceExpression;
            if (mre != null)
            {
                // TODO: method mapping

                foreach (var t in mre.TypeArguments)
                {
                    inv.TypeArguments.Add(GetTypeReference(t));
                }

                inv.Type = new E.PropertyExpression()
                {
                    Object = GetExpression(mre.Target, data),
                    Property = mre.MemberName
                };
            }
            else
            {
                inv.Type = GetExpression(invocationExpression.Target, data);
            }

            foreach (var p in invocationExpression.Arguments)
            {
                inv.Parameters.Add(GetExpression(p, data));
            }

            yield return inv;
        }


        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, string data)
        {
            if (memberReferenceExpression.TypeArguments.Any())
            {
                throw new NotImplementedException();
            }

            yield return new E.PropertyExpression()
            {
                Object = GetExpression(memberReferenceExpression.Target, data),
                Property = memberReferenceExpression.MemberName
            };
        }

        private Expression GetExpression(ICSharpCode.NRefactory.CSharp.Expression expression, string data)
               => expression?.IsNull != false ? null : expression.AcceptVisitor(this, data).Cast<Expression>().Single();

        #endregion 式レベル

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitAccessor(Accessor accessor, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitAnonymousTypeCreateExpression(AnonymousTypeCreateExpression anonymousTypeCreateExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitArraySpecifier(ArraySpecifier arraySpecifier, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitAsExpression(AsExpression asExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitAssignmentExpression(AssignmentExpression assignmentExpression, string data)
        {
            throw new NotImplementedException();
        }

        private string GetTypeName(AstType type)
        {
            var tr = type.Annotations.OfType<TypeReference>()?.FirstOrDefault();
            if (tr != null)
            {
                return tr.FullName;
            }
            return ((SimpleType)type).Identifier;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitAttributeSection(AttributeSection attributeSection, string data)
        {
            foreach (var c in attributeSection.Children)
            {
                foreach (var cr in c.AcceptVisitor(this, data))
                {
                    yield return cr;
                }
            }
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitBlockStatement(BlockStatement blockStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitBreakStatement(BreakStatement breakStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitCaseLabel(CaseLabel caseLabel, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitCastExpression(CastExpression castExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitCatchClause(CatchClause catchClause, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitCheckedExpression(CheckedExpression checkedExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitCheckedStatement(CheckedStatement checkedStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitComment(Comment comment, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitComposedType(ComposedType composedType, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitConditionalExpression(ConditionalExpression conditionalExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitConstraint(Constraint constraint, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitConstructorInitializer(ConstructorInitializer constructorInitializer, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitContinueStatement(ContinueStatement continueStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitCSharpTokenNode(CSharpTokenNode cSharpTokenNode, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitCustomEventDeclaration(CustomEventDeclaration customEventDeclaration, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitDirectionExpression(DirectionExpression directionExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitDocumentationReference(DocumentationReference documentationReference, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitDoWhileStatement(DoWhileStatement doWhileStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitEmptyStatement(EmptyStatement emptyStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitErrorNode(AstNode errorNode, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitEventDeclaration(EventDeclaration eventDeclaration, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitExpressionStatement(ExpressionStatement expressionStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitExternAliasDeclaration(ExternAliasDeclaration externAliasDeclaration, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitFixedStatement(FixedStatement fixedStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitFixedVariableInitializer(FixedVariableInitializer fixedVariableInitializer, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitForeachStatement(ForeachStatement foreachStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitForStatement(ForStatement forStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitGotoDefaultStatement(GotoDefaultStatement gotoDefaultStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitGotoStatement(GotoStatement gotoStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitIdentifier(Identifier identifier, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitIdentifierExpression(IdentifierExpression identifierExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitIndexerExpression(IndexerExpression indexerExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitIsExpression(IsExpression isExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitLabelStatement(LabelStatement labelStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitLambdaExpression(LambdaExpression lambdaExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitLockStatement(LockStatement lockStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitMemberType(MemberType memberType, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitNamedExpression(NamedExpression namedExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitNewLine(NewLineNode newLineNode, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitNullNode(AstNode nullNode, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitNullReferenceExpression(NullReferenceExpression nullReferenceExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitParameterDeclaration(ParameterDeclaration parameterDeclaration, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitPatternPlaceholder(AstNode placeholder, Pattern pattern, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitPreProcessorDirective(PreProcessorDirective preProcessorDirective, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, string data)
        {
            if (primitiveExpression.Value == null)
            {
                yield return new E.NullExpression();
            }
            else
            {
                var sv = primitiveExpression.Value as string;
                if (sv != null)
                {
                    yield return new E.StringExpression() { Value = sv };
                }
                else if (primitiveExpression.Value is bool)
                {
                    yield return new E.BooleanExpression() { Value = (bool)primitiveExpression.Value };
                }
                else
                {
                    yield return new E.NumberExpression()
                    {
                        Value = Convert.ToDouble(primitiveExpression.Value)
                    };
                }
            }
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitPrimitiveType(PrimitiveType primitiveType, string data)
        {
            throw new NotImplementedException();
        }

        private Collection<Statement> GetStatements(string data, BlockStatement body)
        {
            Collection<Statement> sts = null;
            foreach (var s in body)
            {
                foreach (var cr in s.AcceptVisitor(this, data))
                {
                    (sts ?? (sts = new Collection<Statement>())).Add((Statement)cr);
                }
            }

            return sts;
        }

        private static D.AccessibilityModifier GetAccessibility(Modifiers m)
            => (m & (Modifiers.Public | Modifiers.Internal)) != Modifiers.None ? D.AccessibilityModifier.Public
                : (m & Modifiers.Protected) != Modifiers.None ? D.AccessibilityModifier.Protected
                : D.AccessibilityModifier.Private;

        private ITypeReference GetTypeReference(AstType type)
        {
            bool b;
            return GetTypeReference(type, out b);
        }

        private ITypeReference GetTypeReference(AstType type, out bool isOptional)
        {
            var t = type.Annotations?.OfType<Type>().FirstOrDefault();
            if (t != null)
            {
                var ut = t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>) ? t.GetGenericArguments()[0] : t;
                isOptional = !t.IsValueType || ut != t;

                if (ut == typeof(void))
                {
                    return D.PredefinedType.Void;
                }

                if (ut == typeof(object))
                {
                    return D.PredefinedType.Any;
                }

                if (ut == typeof(bool))
                {
                    return D.PredefinedType.Boolean;
                }

                if (ut == typeof(int)
                    || ut == typeof(float)
                    || ut == typeof(double)
                    || ut == typeof(decimal)
                    || ut == typeof(long)
                    || ut == typeof(byte)
                    || ut == typeof(short)
                    || ut == typeof(uint)
                    || ut == typeof(ulong)
                    || ut == typeof(sbyte)
                    || ut == typeof(ulong))
                {
                    return D.PredefinedType.Number;
                }

                if (ut == typeof(string))
                {
                    return D.PredefinedType.String;
                }
            }

            isOptional = true;

            var pt = type as PrimitiveType;

            if (pt != null)
            {
                switch (pt.Keyword)
                {
                    case "void":
                        return D.PredefinedType.Void;

                    case "object":
                        return D.PredefinedType.Any;

                    case "bool":
                        isOptional = false;
                        return D.PredefinedType.Boolean;

                    case "byte":
                    case "sbyte":
                    case "short":
                    case "ushort":
                    case "int":
                    case "uint":
                    case "long":
                    case "ulong":
                    case "float":
                    case "double":
                    case "decimal":
                        isOptional = false;
                        return D.PredefinedType.Number;

                    case "string":
                        return D.PredefinedType.String;

                    default:
                        throw new NotImplementedException();
                }
            }

            // TODO: キャッシュおよびモジュール内の検索

            return new D.NamedTypeReference() { Name = ((SimpleType)type).Identifier };
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitQueryContinuationClause(QueryContinuationClause queryContinuationClause, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitQueryExpression(QueryExpression queryExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitQueryFromClause(QueryFromClause queryFromClause, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitQueryGroupClause(QueryGroupClause queryGroupClause, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitQueryJoinClause(QueryJoinClause queryJoinClause, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitQueryLetClause(QueryLetClause queryLetClause, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitQueryOrderClause(QueryOrderClause queryOrderClause, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitQueryOrdering(QueryOrdering queryOrdering, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitQuerySelectClause(QuerySelectClause querySelectClause, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitQueryWhereClause(QueryWhereClause queryWhereClause, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitSimpleType(SimpleType simpleType, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitSizeOfExpression(SizeOfExpression sizeOfExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitStackAllocExpression(StackAllocExpression stackAllocExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitSwitchSection(SwitchSection switchSection, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitSwitchStatement(SwitchStatement switchStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitSyntaxTree(SyntaxTree syntaxTree, string data)
        {
            foreach (var c in syntaxTree.Children)
            {
                foreach (var s in c.AcceptVisitor(this, data))
                {
                    yield return s;
                }
            }
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitText(TextNode textNode, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitThrowStatement(ThrowStatement throwStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitTryCatchStatement(TryCatchStatement tryCatchStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitTypeOfExpression(TypeOfExpression typeOfExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitTypeParameterDeclaration(TypeParameterDeclaration typeParameterDeclaration, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitUncheckedExpression(UncheckedExpression uncheckedExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitUncheckedStatement(UncheckedStatement uncheckedStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitUndocumentedExpression(UndocumentedExpression undocumentedExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitUnsafeStatement(UnsafeStatement unsafeStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitUsingAliasDeclaration(UsingAliasDeclaration usingAliasDeclaration, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitUsingDeclaration(UsingDeclaration usingDeclaration, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitUsingStatement(UsingStatement usingStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitVariableInitializer(VariableInitializer variableInitializer, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitWhileStatement(WhileStatement whileStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitWhitespace(WhitespaceNode whitespaceNode, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitYieldBreakStatement(YieldBreakStatement yieldBreakStatement, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitYieldReturnStatement(YieldReturnStatement yieldReturnStatement, string data)
        {
            throw new NotImplementedException();
        }
    }
}