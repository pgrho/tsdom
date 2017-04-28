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

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public partial class ClrToTypeScriptTransformer : IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>
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

            return b.SyntaxTree.AcceptVisitor(this, new ClrToTypeScriptTransformationContext()).ToArray();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitAccessor(Accessor accessor, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitAnonymousTypeCreateExpression(AnonymousTypeCreateExpression anonymousTypeCreateExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitArraySpecifier(ArraySpecifier arraySpecifier, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitAsExpression(AsExpression asExpression, ClrToTypeScriptTransformationContext data)
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

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitCaseLabel(CaseLabel caseLabel, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitCastExpression(CastExpression castExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitCatchClause(CatchClause catchClause, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitCheckedExpression(CheckedExpression checkedExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitCheckedStatement(CheckedStatement checkedStatement, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitComment(Comment comment, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitComposedType(ComposedType composedType, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitConditionalExpression(ConditionalExpression conditionalExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitConstraint(Constraint constraint, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitConstructorInitializer(ConstructorInitializer constructorInitializer, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitCSharpTokenNode(CSharpTokenNode cSharpTokenNode, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitCustomEventDeclaration(CustomEventDeclaration customEventDeclaration, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitDirectionExpression(DirectionExpression directionExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitDocumentationReference(DocumentationReference documentationReference, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitErrorNode(AstNode errorNode, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitEventDeclaration(EventDeclaration eventDeclaration, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitExternAliasDeclaration(ExternAliasDeclaration externAliasDeclaration, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitFixedStatement(FixedStatement fixedStatement, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitFixedVariableInitializer(FixedVariableInitializer fixedVariableInitializer, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitGotoDefaultStatement(GotoDefaultStatement gotoDefaultStatement, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitGotoStatement(GotoStatement gotoStatement, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitIdentifier(Identifier identifier, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitIsExpression(IsExpression isExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitLabelStatement(LabelStatement labelStatement, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitLambdaExpression(LambdaExpression lambdaExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitMemberType(MemberType memberType, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitNamedExpression(NamedExpression namedExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitNewLine(NewLineNode newLineNode, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitNullNode(AstNode nullNode, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitParameterDeclaration(ParameterDeclaration parameterDeclaration, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitPatternPlaceholder(AstNode placeholder, Pattern pattern, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, ClrToTypeScriptTransformationContext data)
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

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitPrimitiveType(PrimitiveType primitiveType, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        private Collection<Statement> GetStatements(AstNodeCollection<ICSharpCode.NRefactory.CSharp.Statement> statements, ClrToTypeScriptTransformationContext data)
        {
            Collection<Statement> sts = null;

            foreach (var s in statements)
            {
                var l = GetStatements(s, data);

                if (l?.Count > 0)
                {
                    if (sts == null)
                    {
                        sts = l;
                    }
                    else
                    {
                        foreach (var e in l)
                        {
                            sts.Add(e);
                        }
                    }
                }
            }

            return sts;
        }

        private Collection<Statement> GetStatements(ICSharpCode.NRefactory.CSharp.Statement statement, ClrToTypeScriptTransformationContext data)
        {
            Collection<Statement> sts = null;

            var block = statement as BlockStatement;
            if (block != null)
            {
                foreach (var s in block.Statements)
                {
                    foreach (var cr in s.AcceptVisitor(this, data))
                    {
                        (sts ?? (sts = new Collection<Statement>())).Add((Statement)cr);
                    }
                }
            }
            else if (statement?.IsNull == false)
            {
                foreach (var cr in statement.AcceptVisitor(this, data))
                {
                    (sts ?? (sts = new Collection<Statement>())).Add((Statement)cr);
                }
            }
            return sts;
        }

        private static D.AccessibilityModifier GetAccessibility(Modifiers modifiers)
            => (modifiers & (Modifiers.Public | Modifiers.Internal)) != Modifiers.None ? D.AccessibilityModifier.Public
                : (modifiers & Modifiers.Protected) != Modifiers.None ? D.AccessibilityModifier.Protected
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
                return GetTypeReference(t, out isOptional);
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

            var ct = type as ComposedType;

            if (ct != null)
            {
                return new D.ArrayType()
                {
                    ElementType = GetTypeReference(ct.BaseType)
                };
            }

            // TODO: キャッシュおよびモジュール内の検索

            return new D.NamedTypeReference() { Name = ((SimpleType)type).Identifier };
        }

        private ITypeReference GetTypeReference(Type type)
        {
            bool b;
            return GetTypeReference(type, out b);
        }

        private ITypeReference GetTypeReference(Type type, out bool isOptional)
        {
            if (type.IsArray)
            {
                if (type.GetArrayRank() != 1)
                {
                    throw new NotSupportedException();
                }

                isOptional = true;
                return new D.ArrayType()
                {
                    ElementType = GetTypeReference(type.GetElementType())
                };
            }

            var ut = type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) ? type.GetGenericArguments()[0] : type;
            isOptional = !type.IsValueType || ut != type;

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

            return new D.NamedTypeReference() { Name = type.FullName };
        }

        #region クエリー

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitQueryContinuationClause(QueryContinuationClause queryContinuationClause, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitQueryExpression(QueryExpression queryExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitQueryFromClause(QueryFromClause queryFromClause, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitQueryGroupClause(QueryGroupClause queryGroupClause, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitQueryJoinClause(QueryJoinClause queryJoinClause, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitQueryLetClause(QueryLetClause queryLetClause, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitQueryOrderClause(QueryOrderClause queryOrderClause, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitQueryOrdering(QueryOrdering queryOrdering, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitQuerySelectClause(QuerySelectClause querySelectClause, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitQueryWhereClause(QueryWhereClause queryWhereClause, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        #endregion クエリー

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitSimpleType(SimpleType simpleType, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitSizeOfExpression(SizeOfExpression sizeOfExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitStackAllocExpression(StackAllocExpression stackAllocExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitSwitchSection(SwitchSection switchSection, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitSyntaxTree(SyntaxTree syntaxTree, ClrToTypeScriptTransformationContext data)
        {
            foreach (var c in syntaxTree.Children)
            {
                foreach (var s in c.AcceptVisitor(this, data))
                {
                    yield return s;
                }
            }
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitText(TextNode textNode, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitThrowStatement(ThrowStatement throwStatement, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitTypeOfExpression(TypeOfExpression typeOfExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitUncheckedExpression(UncheckedExpression uncheckedExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitUncheckedStatement(UncheckedStatement uncheckedStatement, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitUndocumentedExpression(UndocumentedExpression undocumentedExpression, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitUnsafeStatement(UnsafeStatement unsafeStatement, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitVariableInitializer(VariableInitializer variableInitializer, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitWhitespace(WhitespaceNode whitespaceNode, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitYieldBreakStatement(YieldBreakStatement yieldBreakStatement, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ClrToTypeScriptTransformationContext, IEnumerable<Syntax>>.VisitYieldReturnStatement(YieldReturnStatement yieldReturnStatement, ClrToTypeScriptTransformationContext data)
        {
            throw new NotImplementedException();
        }
    }
}