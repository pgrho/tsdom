using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.PatternMatching;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using D = Shipwreck.TypeScriptModels.Declarations;
using E = Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public partial class ILTranslator : IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>
    {
        private List<D.IRootStatement> _Statements;

        public List<D.IRootStatement> Statements
            => _Statements ?? (_Statements = new List<D.IRootStatement>());

        private static ILTranslationConvention[] _DefaultConventions;


        private List<ILTranslationConvention> _Conventions;

        public List<ILTranslationConvention> Conventions
            => _Conventions ?? (_Conventions = DefaultConventions.ToList());

        private CSharpProjectContent _Project;

        internal CSharpProjectContent Project
            => _Project ?? (_Project = new CSharpProjectContent());

        public static ILTranslationConvention[] DefaultConventions
        {
            get
            {
                if (_DefaultConventions == null)
                {
                    _DefaultConventions = new[]
                    {
                        new MethodNameConvention(typeof(object).GetMethod(nameof(ToString)), "toString")
                    };
                }
                return _DefaultConventions;
            }
        }

        public IEnumerable<Syntax> Transform(Type clrType)
        {
            using (var ar = new AppDomainAssemblyResolver())
            using (var fs = new FileStream(clrType.Assembly.Location, FileMode.Open, FileAccess.Read))
            {
                var ad = AssemblyDefinition.ReadAssembly(fs, new ReaderParameters()
                {
                    AssemblyResolver = ar
                });
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
                b.SyntaxTree.FileName = "temp.cs";

                foreach (var c in Conventions)
                {
                    c.ApplyTo(this);
                }

                return b.SyntaxTree.AcceptVisitor(this, new ILTransformationContext(this, b.SyntaxTree)).ToArray();
            }
        }

        private IEnumerable<Syntax> OnVisiting<T>(ILTransformationContext data, T node, EventHandler<TranslationEventArgs<T>> handler)
            where T : AstNode
        {
            if (handler != null)
            {
                var e = TranslationEventArgs.Create(data, node);
                handler(this, e);
                if (e.Results != null)
                {
                    return e.Results;
                }
            }

            return null;
        }

        private IEnumerable<Syntax> OnVisited<T>(ILTransformationContext data, T node, EventHandler<TranslationEventArgs<T>> handler, Syntax result)
            where T : AstNode
            => OnVisited(data, node, handler, new[] { result });

        private IEnumerable<Syntax> OnVisited<T>(ILTransformationContext data, T node, EventHandler<TranslationEventArgs<T>> handler, IEnumerable<Syntax> results)
            where T : AstNode
        {
            if (handler != null)
            {
                var e = TranslationEventArgs.Create(data, node, results);
                handler(this, e);
                if (e.Results != null)
                {
                    return e.Results;
                }
            }

            return results;
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitAccessor(Accessor accessor, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitAnonymousTypeCreateExpression(AnonymousTypeCreateExpression anonymousTypeCreateExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitArraySpecifier(ArraySpecifier arraySpecifier, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitAsExpression(AsExpression asExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
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

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitCaseLabel(CaseLabel caseLabel, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitCastExpression(CastExpression castExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitCatchClause(CatchClause catchClause, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitCheckedExpression(CheckedExpression checkedExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitCheckedStatement(CheckedStatement checkedStatement, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitComment(Comment comment, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitComposedType(ComposedType composedType, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitConditionalExpression(ConditionalExpression conditionalExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitConstraint(Constraint constraint, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitConstructorInitializer(ConstructorInitializer constructorInitializer, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitCSharpTokenNode(CSharpTokenNode cSharpTokenNode, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitCustomEventDeclaration(CustomEventDeclaration customEventDeclaration, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitDirectionExpression(DirectionExpression directionExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitDocumentationReference(DocumentationReference documentationReference, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitErrorNode(AstNode errorNode, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitEventDeclaration(EventDeclaration eventDeclaration, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitExternAliasDeclaration(ExternAliasDeclaration externAliasDeclaration, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitFixedStatement(FixedStatement fixedStatement, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitFixedVariableInitializer(FixedVariableInitializer fixedVariableInitializer, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitGotoDefaultStatement(GotoDefaultStatement gotoDefaultStatement, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitGotoStatement(GotoStatement gotoStatement, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitIdentifier(Identifier identifier, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitIsExpression(IsExpression isExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitLabelStatement(LabelStatement labelStatement, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitLambdaExpression(LambdaExpression lambdaExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitMemberType(MemberType memberType, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitNamedExpression(NamedExpression namedExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitNewLine(NewLineNode newLineNode, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitNullNode(AstNode nullNode, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitParameterDeclaration(ParameterDeclaration parameterDeclaration, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitPatternPlaceholder(AstNode placeholder, Pattern pattern, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, ILTransformationContext data)
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

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitPrimitiveType(PrimitiveType primitiveType, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        private Collection<Statement> GetStatements(AstNodeCollection<ICSharpCode.NRefactory.CSharp.Statement> statements, ILTransformationContext data)
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

        private Collection<Statement> GetStatements(ICSharpCode.NRefactory.CSharp.Statement statement, ILTransformationContext data)
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
                        throw GetNotImplementedException();
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

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryContinuationClause(QueryContinuationClause queryContinuationClause, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryExpression(QueryExpression queryExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryFromClause(QueryFromClause queryFromClause, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryGroupClause(QueryGroupClause queryGroupClause, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryJoinClause(QueryJoinClause queryJoinClause, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryLetClause(QueryLetClause queryLetClause, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryOrderClause(QueryOrderClause queryOrderClause, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryOrdering(QueryOrdering queryOrdering, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQuerySelectClause(QuerySelectClause querySelectClause, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryWhereClause(QueryWhereClause queryWhereClause, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        #endregion クエリー

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitSimpleType(SimpleType simpleType, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitSizeOfExpression(SizeOfExpression sizeOfExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitStackAllocExpression(StackAllocExpression stackAllocExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitSwitchSection(SwitchSection switchSection, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitSyntaxTree(SyntaxTree syntaxTree, ILTransformationContext data)
        {
            foreach (var c in syntaxTree.Children)
            {
                foreach (var s in c.AcceptVisitor(this, data))
                {
                    yield return s;
                }
            }
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitText(TextNode textNode, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitThrowStatement(ThrowStatement throwStatement, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitTypeOfExpression(TypeOfExpression typeOfExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitUncheckedExpression(UncheckedExpression uncheckedExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitUncheckedStatement(UncheckedStatement uncheckedStatement, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitUndocumentedExpression(UndocumentedExpression undocumentedExpression, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitUnsafeStatement(UnsafeStatement unsafeStatement, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitVariableInitializer(VariableInitializer variableInitializer, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitWhitespace(WhitespaceNode whitespaceNode, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitYieldBreakStatement(YieldBreakStatement yieldBreakStatement, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitYieldReturnStatement(YieldReturnStatement yieldReturnStatement, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        private static NotImplementedException GetNotImplementedException([CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
            => new NotImplementedException($"{memberName}は実装されていません。{Path.GetFileName(filePath)}@{lineNumber}");
    }
}