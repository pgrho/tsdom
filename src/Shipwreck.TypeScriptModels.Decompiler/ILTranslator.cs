using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using D = Shipwreck.TypeScriptModels.Declarations;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public partial class ILTranslator : IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>
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
                    _DefaultConventions = new ILTranslationConvention[]
                    {
                        new MethodNameConvention(typeof(object).GetMethod(nameof(ToString)), "toString"),
                        new EventConvention(),
                        new DelegateConvention(),
                        new MathConventionSet()
                    };
                }
                return _DefaultConventions;
            }
        }

        public IEnumerable<Syntax> Translate(Type clrType)
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
                foreach (var a in clrType.Assembly.GetReferencedAssemblies())
                {
                    if (!Project.AssemblyReferences.OfType<DefaultAssemblyReference>().Any(e => a.Name.Equals(e.ToString())))
                    {
                        Project.AddAssemblyReferences(new DefaultAssemblyReference(a.FullName));
                    }
                }

                b.DecompileMethodBodies = true;

                b.AddType(ad.MainModule.GetType(clrType.FullName.Replace('+', '/')));
                b.RunTransformations();
                b.SyntaxTree.FileName = "temp.cs";

                foreach (var c in Conventions)
                {
                    c.ApplyTo(this);
                }

                return b.SyntaxTree.AcceptVisitor(this, new ILTranslationContext(this, b.SyntaxTree)).ToArray();
            }
        }

        public event EventHandler<VisitingEventArgs<AstNode>> VisitingNode;

        private IEnumerable<Syntax> OnVisiting<T>(ILTranslationContext data, T node, EventHandler<VisitingEventArgs<T>> handler)
            where T : AstNode
        {
            var nodeHandler = VisitingNode;

            if (nodeHandler != null)
            {
                var e = new VisitingEventArgs<AstNode>(data, node);
                nodeHandler(this, e);
                if (e.Results != null)
                {
                    return e.Results;
                }
            }

            if (handler != null)
            {
                var e = new VisitingEventArgs<T>(data, node);
                handler(this, e);
                if (e.Results != null)
                {
                    return e.Results;
                }
            }

            return null;
        }

        private IEnumerable<Syntax> OnVisited<T>(ILTranslationContext data, T node, EventHandler<VisitedEventArgs<T>> handler, Syntax result)
            where T : AstNode
            => OnVisited(data, node, handler, new[] { result });

        private IEnumerable<Syntax> OnVisited<T>(ILTranslationContext data, T node, EventHandler<VisitedEventArgs<T>> handler, IEnumerable<Syntax> results)
            where T : AstNode
        {
            if (handler != null)
            {
                var e = new VisitedEventArgs<T>(data, node, results);
                handler(this, e);
                return e.Results ?? results;
            }

            return results;
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitAccessor(Accessor accessor, ILTranslationContext data)
            => OnVisiting(data, accessor, VisitingAccessor)
            ?? OnVisited(data, accessor, VisitedAccessor, TranslateAccessor(accessor, data));

        protected virtual IEnumerable<Syntax> TranslateAccessor(Accessor accessor, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(Accessor));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, ILTranslationContext data)
            => OnVisiting(data, anonymousMethodExpression, VisitingAnonymousMethodExpression)
            ?? OnVisited(data, anonymousMethodExpression, VisitedAnonymousMethodExpression, TranslateAnonymousMethodExpression(anonymousMethodExpression, data));

        protected virtual IEnumerable<Syntax> TranslateAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(AnonymousMethodExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitAnonymousTypeCreateExpression(AnonymousTypeCreateExpression anonymousTypeCreateExpression, ILTranslationContext data)
            => OnVisiting(data, anonymousTypeCreateExpression, VisitingAnonymousTypeCreateExpression)
            ?? OnVisited(data, anonymousTypeCreateExpression, VisitedAnonymousTypeCreateExpression, TranslateAnonymousTypeCreateExpression(anonymousTypeCreateExpression, data));

        protected virtual IEnumerable<Syntax> TranslateAnonymousTypeCreateExpression(AnonymousTypeCreateExpression anonymousTypeCreateExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(AnonymousTypeCreateExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, ILTranslationContext data)
            => OnVisiting(data, arrayCreateExpression, VisitingArrayCreateExpression)
            ?? OnVisited(data, arrayCreateExpression, VisitedArrayCreateExpression, TranslateArrayCreateExpression(arrayCreateExpression, data));

        protected virtual IEnumerable<Syntax> TranslateArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ArrayCreateExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, ILTranslationContext data)
            => OnVisiting(data, arrayInitializerExpression, VisitingArrayInitializerExpression)
            ?? OnVisited(data, arrayInitializerExpression, VisitedArrayInitializerExpression, TranslateArrayInitializerExpression(arrayInitializerExpression, data));

        protected virtual IEnumerable<Syntax> TranslateArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ArrayInitializerExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitArraySpecifier(ArraySpecifier arraySpecifier, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ArraySpecifier));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitAsExpression(AsExpression asExpression, ILTranslationContext data)
            => OnVisiting(data, asExpression, VisitingAsExpression)
            ?? OnVisited(data, asExpression, VisitedAsExpression, TranslateAsExpression(asExpression, data));

        protected virtual IEnumerable<Syntax> TranslateAsExpression(AsExpression asExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(AsExpression));
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

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitCaseLabel(CaseLabel caseLabel, ILTranslationContext data)
            => OnVisiting(data, caseLabel, VisitingCaseLabel)
            ?? OnVisited(data, caseLabel, VisitedCaseLabel, TranslateCaseLabel(caseLabel, data));

        protected virtual IEnumerable<Syntax> TranslateCaseLabel(CaseLabel caseLabel, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(CaseLabel));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitCastExpression(CastExpression castExpression, ILTranslationContext data)
            => OnVisiting(data, castExpression, VisitingCastExpression)
            ?? OnVisited(data, castExpression, VisitedCastExpression, TranslateCastExpression(castExpression, data));

        protected virtual IEnumerable<Syntax> TranslateCastExpression(CastExpression castExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(CastExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitCatchClause(CatchClause catchClause, ILTranslationContext data)
            => OnVisiting(data, catchClause, VisitingCatchClause)
            ?? OnVisited(data, catchClause, VisitedCatchClause, TranslateCatchClause(catchClause, data));

        protected virtual IEnumerable<Syntax> TranslateCatchClause(CatchClause catchClause, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(CatchClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitCheckedExpression(CheckedExpression checkedExpression, ILTranslationContext data)
            => OnVisiting(data, checkedExpression, VisitingCheckedExpression)
            ?? OnVisited(data, checkedExpression, VisitedCheckedExpression, TranslateCheckedExpression(checkedExpression, data));

        protected virtual IEnumerable<Syntax> TranslateCheckedExpression(CheckedExpression checkedExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(CheckedExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitCheckedStatement(CheckedStatement checkedStatement, ILTranslationContext data)
            => OnVisiting(data, checkedStatement, VisitingCheckedStatement)
            ?? OnVisited(data, checkedStatement, VisitedCheckedStatement, TranslateCheckedStatement(checkedStatement, data));

        protected virtual IEnumerable<Syntax> TranslateCheckedStatement(CheckedStatement checkedStatement, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(CheckedStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitComment(Comment comment, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(Comment));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitComposedType(ComposedType composedType, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ComposedType));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitConditionalExpression(ConditionalExpression conditionalExpression, ILTranslationContext data)
            => OnVisiting(data, conditionalExpression, VisitingConditionalExpression)
            ?? OnVisited(data, conditionalExpression, VisitedConditionalExpression, TranslateConditionalExpression(conditionalExpression, data));

        protected virtual IEnumerable<Syntax> TranslateConditionalExpression(ConditionalExpression conditionalExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ConditionalExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitConstraint(Constraint constraint, ILTranslationContext data)
            => OnVisiting(data, constraint, VisitingConstraint)
            ?? OnVisited(data, constraint, VisitedConstraint, TranslateConstraint(constraint, data));

        protected virtual IEnumerable<Syntax> TranslateConstraint(Constraint constraint, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(Constraint));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitConstructorInitializer(ConstructorInitializer constructorInitializer, ILTranslationContext data)
            => OnVisiting(data, constructorInitializer, VisitingConstructorInitializer)
            ?? OnVisited(data, constructorInitializer, VisitedConstructorInitializer, TranslateConstructorInitializer(constructorInitializer, data));

        protected virtual IEnumerable<Syntax> TranslateConstructorInitializer(ConstructorInitializer constructorInitializer, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ConstructorInitializer));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitCSharpTokenNode(CSharpTokenNode cSharpTokenNode, ILTranslationContext data)
            => OnVisiting(data, cSharpTokenNode, VisitingCSharpTokenNode)
            ?? OnVisited(data, cSharpTokenNode, VisitedCSharpTokenNode, TranslateCSharpTokenNode(cSharpTokenNode, data));

        protected virtual IEnumerable<Syntax> TranslateCSharpTokenNode(CSharpTokenNode cSharpTokenNode, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(CSharpTokenNode));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, ILTranslationContext data)
            => OnVisiting(data, delegateDeclaration, VisitingDelegateDeclaration)
            ?? OnVisited(data, delegateDeclaration, VisitedDelegateDeclaration, TranslateDelegateDeclaration(delegateDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateDelegateDeclaration(DelegateDeclaration delegateDeclaration, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(DelegateDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, ILTranslationContext data)
            => OnVisiting(data, destructorDeclaration, VisitingDestructorDeclaration)
            ?? OnVisited(data, destructorDeclaration, VisitedDestructorDeclaration, TranslateDestructorDeclaration(destructorDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateDestructorDeclaration(DestructorDeclaration destructorDeclaration, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(DestructorDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitDirectionExpression(DirectionExpression directionExpression, ILTranslationContext data)
            => OnVisiting(data, directionExpression, VisitingDirectionExpression)
            ?? OnVisited(data, directionExpression, VisitedDirectionExpression, TranslateDirectionExpression(directionExpression, data));

        protected virtual IEnumerable<Syntax> TranslateDirectionExpression(DirectionExpression directionExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(DirectionExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitDocumentationReference(DocumentationReference documentationReference, ILTranslationContext data)
            => OnVisiting(data, documentationReference, VisitingDocumentationReference)
            ?? OnVisited(data, documentationReference, VisitedDocumentationReference, TranslateDocumentationReference(documentationReference, data));

        protected virtual IEnumerable<Syntax> TranslateDocumentationReference(DocumentationReference documentationReference, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(DocumentationReference));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration, ILTranslationContext data)
            => OnVisiting(data, enumMemberDeclaration, VisitingEnumMemberDeclaration)
            ?? OnVisited(data, enumMemberDeclaration, VisitedEnumMemberDeclaration, TranslateEnumMemberDeclaration(enumMemberDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(EnumMemberDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitErrorNode(AstNode errorNode, ILTranslationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitExternAliasDeclaration(ExternAliasDeclaration externAliasDeclaration, ILTranslationContext data)
            => OnVisiting(data, externAliasDeclaration, VisitingExternAliasDeclaration)
            ?? OnVisited(data, externAliasDeclaration, VisitedExternAliasDeclaration, TranslateExternAliasDeclaration(externAliasDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateExternAliasDeclaration(ExternAliasDeclaration externAliasDeclaration, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ExternAliasDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration, ILTranslationContext data)
            => OnVisiting(data, fixedFieldDeclaration, VisitingFixedFieldDeclaration)
            ?? OnVisited(data, fixedFieldDeclaration, VisitedFixedFieldDeclaration, TranslateFixedFieldDeclaration(fixedFieldDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(FixedFieldDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitFixedStatement(FixedStatement fixedStatement, ILTranslationContext data)
            => OnVisiting(data, fixedStatement, VisitingFixedStatement)
            ?? OnVisited(data, fixedStatement, VisitedFixedStatement, TranslateFixedStatement(fixedStatement, data));

        protected virtual IEnumerable<Syntax> TranslateFixedStatement(FixedStatement fixedStatement, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(FixedStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitFixedVariableInitializer(FixedVariableInitializer fixedVariableInitializer, ILTranslationContext data)
            => OnVisiting(data, fixedVariableInitializer, VisitingFixedVariableInitializer)
            ?? OnVisited(data, fixedVariableInitializer, VisitedFixedVariableInitializer, TranslateFixedVariableInitializer(fixedVariableInitializer, data));

        protected virtual IEnumerable<Syntax> TranslateFixedVariableInitializer(FixedVariableInitializer fixedVariableInitializer, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(FixedVariableInitializer));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, ILTranslationContext data)
            => OnVisiting(data, gotoCaseStatement, VisitingGotoCaseStatement)
            ?? OnVisited(data, gotoCaseStatement, VisitedGotoCaseStatement, TranslateGotoCaseStatement(gotoCaseStatement, data));

        protected virtual IEnumerable<Syntax> TranslateGotoCaseStatement(GotoCaseStatement gotoCaseStatement, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(GotoCaseStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitGotoDefaultStatement(GotoDefaultStatement gotoDefaultStatement, ILTranslationContext data)
            => OnVisiting(data, gotoDefaultStatement, VisitingGotoDefaultStatement)
            ?? OnVisited(data, gotoDefaultStatement, VisitedGotoDefaultStatement, TranslateGotoDefaultStatement(gotoDefaultStatement, data));

        protected virtual IEnumerable<Syntax> TranslateGotoDefaultStatement(GotoDefaultStatement gotoDefaultStatement, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(GotoDefaultStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitGotoStatement(GotoStatement gotoStatement, ILTranslationContext data)
            => OnVisiting(data, gotoStatement, VisitingGotoStatement)
            ?? OnVisited(data, gotoStatement, VisitedGotoStatement, TranslateGotoStatement(gotoStatement, data));

        protected virtual IEnumerable<Syntax> TranslateGotoStatement(GotoStatement gotoStatement, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(GotoStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitIdentifier(Identifier identifier, ILTranslationContext data)
            => OnVisiting(data, identifier, VisitingIdentifier)
            ?? OnVisited(data, identifier, VisitedIdentifier, TranslateIdentifier(identifier, data));

        protected virtual IEnumerable<Syntax> TranslateIdentifier(Identifier identifier, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(Identifier));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, ILTranslationContext data)
            => OnVisiting(data, indexerDeclaration, VisitingIndexerDeclaration)
            ?? OnVisited(data, indexerDeclaration, VisitedIndexerDeclaration, TranslateIndexerDeclaration(indexerDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateIndexerDeclaration(IndexerDeclaration indexerDeclaration, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(IndexerDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitIsExpression(IsExpression isExpression, ILTranslationContext data)
            => OnVisiting(data, isExpression, VisitingIsExpression)
            ?? OnVisited(data, isExpression, VisitedIsExpression, TranslateIsExpression(isExpression, data));

        protected virtual IEnumerable<Syntax> TranslateIsExpression(IsExpression isExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(IsExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitLabelStatement(LabelStatement labelStatement, ILTranslationContext data)
            => OnVisiting(data, labelStatement, VisitingLabelStatement)
            ?? OnVisited(data, labelStatement, VisitedLabelStatement, TranslateLabelStatement(labelStatement, data));

        protected virtual IEnumerable<Syntax> TranslateLabelStatement(LabelStatement labelStatement, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(LabelStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitLambdaExpression(LambdaExpression lambdaExpression, ILTranslationContext data)
            => OnVisiting(data, lambdaExpression, VisitingLambdaExpression)
            ?? OnVisited(data, lambdaExpression, VisitedLambdaExpression, TranslateLambdaExpression(lambdaExpression, data));

        protected virtual IEnumerable<Syntax> TranslateLambdaExpression(LambdaExpression lambdaExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(LambdaExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitMemberType(MemberType memberType, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(MemberType));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, ILTranslationContext data)
            => OnVisiting(data, namedArgumentExpression, VisitingNamedArgumentExpression)
            ?? OnVisited(data, namedArgumentExpression, VisitedNamedArgumentExpression, TranslateNamedArgumentExpression(namedArgumentExpression, data));

        protected virtual IEnumerable<Syntax> TranslateNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(NamedArgumentExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitNamedExpression(NamedExpression namedExpression, ILTranslationContext data)
            => OnVisiting(data, namedExpression, VisitingNamedExpression)
            ?? OnVisited(data, namedExpression, VisitedNamedExpression, TranslateNamedExpression(namedExpression, data));

        protected virtual IEnumerable<Syntax> TranslateNamedExpression(NamedExpression namedExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(NamedExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitNewLine(NewLineNode newLineNode, ILTranslationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitNullNode(AstNode nullNode, ILTranslationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, ILTranslationContext data)
            => OnVisiting(data, operatorDeclaration, VisitingOperatorDeclaration)
            ?? OnVisited(data, operatorDeclaration, VisitedOperatorDeclaration, TranslateOperatorDeclaration(operatorDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateOperatorDeclaration(OperatorDeclaration operatorDeclaration, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(OperatorDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitParameterDeclaration(ParameterDeclaration parameterDeclaration, ILTranslationContext data)
            => OnVisiting(data, parameterDeclaration, VisitingParameterDeclaration)
            ?? OnVisited(data, parameterDeclaration, VisitedParameterDeclaration, TranslateParameterDeclaration(parameterDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateParameterDeclaration(ParameterDeclaration parameterDeclaration, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ParameterDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitPatternPlaceholder(AstNode placeholder, Pattern pattern, ILTranslationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, ILTranslationContext data)
            => OnVisiting(data, pointerReferenceExpression, VisitingPointerReferenceExpression)
            ?? OnVisited(data, pointerReferenceExpression, VisitedPointerReferenceExpression, TranslatePointerReferenceExpression(pointerReferenceExpression, data));

        protected virtual IEnumerable<Syntax> TranslatePointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(PointerReferenceExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitPrimitiveType(PrimitiveType primitiveType, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(PrimitiveType));
        }

        public StatementCollection GetStatements(AstNodeCollection<ICSharpCode.NRefactory.CSharp.Statement> statements, ILTranslationContext data)
        {
            StatementCollection sts = null;

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

        public StatementCollection GetStatements(ICSharpCode.NRefactory.CSharp.Statement statement, ILTranslationContext data)
        {
            StatementCollection sts = null;

            var block = statement as BlockStatement;
            if (block != null)
            {
                foreach (var s in block.Statements)
                {
                    foreach (var cr in s.AcceptVisitor(this, data))
                    {
                        (sts ?? (sts = new StatementCollection())).Add((Statement)cr);
                    }
                }
            }
            else if (statement?.IsNull == false)
            {
                foreach (var cr in statement.AcceptVisitor(this, data))
                {
                    (sts ?? (sts = new StatementCollection())).Add((Statement)cr);
                }
            }
            return sts;
        }

        private static D.AccessibilityModifier GetAccessibility(Modifiers modifiers)
            => (modifiers & (Modifiers.Public | Modifiers.Internal)) != Modifiers.None ? D.AccessibilityModifier.Public
                : (modifiers & Modifiers.Protected) != Modifiers.None ? D.AccessibilityModifier.Protected
                : D.AccessibilityModifier.Private;

        #region ResolveType

        public event EventHandler<ResolvingTypeEventArgs<AstType>> ResolvingAstType;

        public event EventHandler<ResolvingTypeEventArgs<Type>> ResolvingClrType;

        public event EventHandler<ResolvingTypeEventArgs<Mono.Cecil.TypeReference>> ResolvingCecilType;

        #region ResolveType

        public ITypeReference ResolveType(AstType type)
            => ResolveType(null, type);

        public ITypeReference ResolveType(AstType type, out bool isOptional)
            => ResolveType(null, type, out isOptional);

        public ITypeReference ResolveType(AstNode context, AstType type)
        {
            bool b;
            return ResolveType(context, type, out b);
        }

        public ITypeReference ResolveType(AstNode context, AstType type, out bool isOptional)
        {
            var t = type.Annotations?.OfType<Type>().FirstOrDefault();
            if (t != null)
            {
                return ResolveClrType(context, t, out isOptional);
            }

            var mt = type.Annotations?.OfType<TypeReference>().FirstOrDefault();
            if (mt != null)
            {
                return ResolveCecilType(context, mt, out isOptional);
            }

            var h = ResolvingAstType;
            if (h != null)
            {
                var e = new ResolvingTypeEventArgs<AstType>(context, type);
                h(this, e);
                if (e.Result != null)
                {
                    isOptional = e.IsOptional != false;
                    return e.Result;
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
                        throw GetNotImplementedException();
                }
            }

            var ct = type as ComposedType;

            if (ct != null)
            {
                return new D.ArrayType()
                {
                    ElementType = ResolveType(context, ct.BaseType)
                };
            }

            // TODO: キャッシュおよびモジュール内の検索

            return new D.NamedTypeReference() { Name = ((SimpleType)type).Identifier };
        }

        #endregion ResolveType

        #region ResolveClrType

        public ITypeReference ResolveClrType(Type type)
            => ResolveClrType(null, type);

        public ITypeReference ResolveClrType(Type type, out bool isOptional)
            => ResolveClrType(null, type, out isOptional);

        public ITypeReference ResolveClrType(AstNode context, Type type)
        {
            bool b;
            return ResolveClrType(context, type, out b);
        }

        public ITypeReference ResolveClrType(AstNode context, Type type, out bool isOptional)
        {
            var h = ResolvingClrType;
            if (h != null)
            {
                var e = new ResolvingTypeEventArgs<Type>(context, type);
                h(this, e);
                if (e.Result != null)
                {
                    isOptional = e.IsOptional != false;
                    return e.Result;
                }
            }

            if (type.IsArray)
            {
                if (type.GetArrayRank() != 1)
                {
                    throw new NotSupportedException();
                }

                isOptional = true;
                return new D.ArrayType()
                {
                    ElementType = ResolveClrType(context, type.GetElementType())
                };
            }

            isOptional = !type.IsValueType;
            if (type.IsGenericType)
            {
                if (!type.IsConstructedGenericType
                    || type.GetGenericTypeDefinition() != typeof(Nullable<>))
                {
                    var gt = new D.NamedTypeReference()
                    {
                        IsClass = type.IsClass,
                        IsInterface = type.IsInterface,
                        IsEnum = type.IsEnum,
                        IsPrimitive = type.IsPrimitive,
                        Name = type.FullName.Split('`')[0]
                    };

                    foreach (var t in type.GetGenericArguments())
                    {
                        gt.TypeArguments.Add(ResolveClrType(context, t));
                    }

                    return gt;
                }
            }

            var ut = type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) ? type.GetGenericArguments()[0] : type;
            isOptional |= ut != type;

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

            return new D.NamedTypeReference()
            {
                IsClass = type.IsClass,
                IsInterface = type.IsInterface,
                IsEnum = type.IsEnum,
                IsPrimitive = type.IsPrimitive,
                Name = type.FullName
            };
        }

        #endregion ResolveClrType

        #region ResolveCecilType

        public ITypeReference ResolveCecilType(TypeReference type)
            => ResolveCecilType(null, type);

        public ITypeReference ResolveCecilType(TypeReference type, out bool isOptional)
            => ResolveCecilType(null, type, out isOptional);

        public ITypeReference ResolveCecilType(AstNode context, TypeReference type)
        {
            bool b;
            return ResolveCecilType(context, type, out b);
        }

        public ITypeReference ResolveCecilType(AstNode context, TypeReference type, out bool isOptional)
        {
            var clr = type.ResolveClrType();
            if (clr != null)
            {
                return ResolveClrType(context, clr, out isOptional);
            }

            var h = ResolvingCecilType;
            if (h != null)
            {
                var e = new ResolvingTypeEventArgs<TypeReference>(context, type);
                h(this, e);
                if (e.Result != null)
                {
                    isOptional = e.IsOptional != false;
                    return e.Result;
                }
            }

            var at = type as ArrayType;
            if (at != null)
            {
                if (at.Rank != 1)
                {
                    throw new NotSupportedException();
                }

                bool b;
                isOptional = true;
                return new D.ArrayType()
                {
                    ElementType = ResolveCecilType(context, at.ElementType, out b)
                };
            }

            var gt = type as GenericInstanceType;
            var isNullable = gt?.Namespace == nameof(System) && type.Name == nameof(Nullable);
            isOptional = !type.IsValueType;
            if (gt != null)
            {
                if (gt.IsDefinition
                    || !isNullable)
                {
                    var r = new D.NamedTypeReference();
                    r.Name = type.FullName.Split('`')[0];

                    foreach (var t in gt.GenericArguments)
                    {
                        bool b;
                        r.TypeArguments.Add(ResolveCecilType(context, t, out b));
                    }

                    return r;
                }
            }

            var ut = isNullable ? gt.GenericArguments[0] : type;

            isOptional |= ut != type;

            if (ut.Namespace == nameof(System))
            {
                switch (ut.Name)
                {
                    case "Void":
                        return D.PredefinedType.Void;

                    case nameof(Object):
                        return D.PredefinedType.Any;

                    case nameof(Boolean):
                        return D.PredefinedType.Boolean;

                    case nameof(String):
                        return D.PredefinedType.String;

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
                        return D.PredefinedType.Number;
                }
            }

            return new D.NamedTypeReference() { Name = type.FullName };
        }

        #endregion ResolveCecilType

        #endregion ResolveType

        #region クエリー

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitQueryContinuationClause(QueryContinuationClause queryContinuationClause, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryContinuationClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitQueryExpression(QueryExpression queryExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitQueryFromClause(QueryFromClause queryFromClause, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryFromClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitQueryGroupClause(QueryGroupClause queryGroupClause, ILTranslationContext data)
            => OnVisiting(data, queryGroupClause, VisitingQueryGroupClause)
            ?? OnVisited(data, queryGroupClause, VisitedQueryGroupClause, TranslateQueryGroupClause(queryGroupClause, data));

        protected virtual IEnumerable<Syntax> TranslateQueryGroupClause(QueryGroupClause queryGroupClause, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryGroupClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitQueryJoinClause(QueryJoinClause queryJoinClause, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryJoinClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitQueryLetClause(QueryLetClause queryLetClause, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryLetClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitQueryOrderClause(QueryOrderClause queryOrderClause, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryOrderClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitQueryOrdering(QueryOrdering queryOrdering, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryOrdering));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitQuerySelectClause(QuerySelectClause querySelectClause, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QuerySelectClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitQueryWhereClause(QueryWhereClause queryWhereClause, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryWhereClause));
        }

        #endregion クエリー

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitSimpleType(SimpleType simpleType, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(SimpleType));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitSizeOfExpression(SizeOfExpression sizeOfExpression, ILTranslationContext data)
            => OnVisiting(data, sizeOfExpression, VisitingSizeOfExpression)
            ?? OnVisited(data, sizeOfExpression, VisitedSizeOfExpression, TranslateSizeOfExpression(sizeOfExpression, data));

        protected virtual IEnumerable<Syntax> TranslateSizeOfExpression(SizeOfExpression sizeOfExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(SizeOfExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitStackAllocExpression(StackAllocExpression stackAllocExpression, ILTranslationContext data)
            => OnVisiting(data, stackAllocExpression, VisitingStackAllocExpression)
            ?? OnVisited(data, stackAllocExpression, VisitedStackAllocExpression, TranslateStackAllocExpression(stackAllocExpression, data));

        protected virtual IEnumerable<Syntax> TranslateStackAllocExpression(StackAllocExpression stackAllocExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(StackAllocExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitSwitchSection(SwitchSection switchSection, ILTranslationContext data)
            => OnVisiting(data, switchSection, VisitingSwitchSection)
            ?? OnVisited(data, switchSection, VisitedSwitchSection, TranslateSwitchSection(switchSection, data));

        protected virtual IEnumerable<Syntax> TranslateSwitchSection(SwitchSection switchSection, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(SwitchSection));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitSyntaxTree(SyntaxTree syntaxTree, ILTranslationContext data)
        {
            foreach (var c in syntaxTree.Children)
            {
                foreach (var s in c.AcceptVisitor(this, data))
                {
                    yield return s;
                }
            }
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitText(TextNode textNode, ILTranslationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitTypeOfExpression(TypeOfExpression typeOfExpression, ILTranslationContext data)
            => OnVisiting(data, typeOfExpression, VisitingTypeOfExpression)
            ?? OnVisited(data, typeOfExpression, VisitedTypeOfExpression, TranslateTypeOfExpression(typeOfExpression, data));

        protected virtual IEnumerable<Syntax> TranslateTypeOfExpression(TypeOfExpression typeOfExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(TypeOfExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitUncheckedExpression(UncheckedExpression uncheckedExpression, ILTranslationContext data)
            => OnVisiting(data, uncheckedExpression, VisitingUncheckedExpression)
            ?? OnVisited(data, uncheckedExpression, VisitedUncheckedExpression, TranslateUncheckedExpression(uncheckedExpression, data));

        protected virtual IEnumerable<Syntax> TranslateUncheckedExpression(UncheckedExpression uncheckedExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(UncheckedExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitUncheckedStatement(UncheckedStatement uncheckedStatement, ILTranslationContext data)
            => OnVisiting(data, uncheckedStatement, VisitingUncheckedStatement)
            ?? OnVisited(data, uncheckedStatement, VisitedUncheckedStatement, TranslateUncheckedStatement(uncheckedStatement, data));

        protected virtual IEnumerable<Syntax> TranslateUncheckedStatement(UncheckedStatement uncheckedStatement, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(UncheckedStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitUndocumentedExpression(UndocumentedExpression undocumentedExpression, ILTranslationContext data)
            => OnVisiting(data, undocumentedExpression, VisitingUndocumentedExpression)
            ?? OnVisited(data, undocumentedExpression, VisitedUndocumentedExpression, TranslateUndocumentedExpression(undocumentedExpression, data));

        protected virtual IEnumerable<Syntax> TranslateUndocumentedExpression(UndocumentedExpression undocumentedExpression, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(UndocumentedExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitUnsafeStatement(UnsafeStatement unsafeStatement, ILTranslationContext data)
            => OnVisiting(data, unsafeStatement, VisitingUnsafeStatement)
            ?? OnVisited(data, unsafeStatement, VisitedUnsafeStatement, TranslateUnsafeStatement(unsafeStatement, data));

        protected virtual IEnumerable<Syntax> TranslateUnsafeStatement(UnsafeStatement unsafeStatement, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(UnsafeStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitVariableInitializer(VariableInitializer variableInitializer, ILTranslationContext data)
            => OnVisiting(data, variableInitializer, VisitingVariableInitializer)
            ?? OnVisited(data, variableInitializer, VisitedVariableInitializer, TranslateVariableInitializer(variableInitializer, data));

        protected virtual IEnumerable<Syntax> TranslateVariableInitializer(VariableInitializer variableInitializer, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(VariableInitializer));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitWhitespace(WhitespaceNode whitespaceNode, ILTranslationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitYieldBreakStatement(YieldBreakStatement yieldBreakStatement, ILTranslationContext data)
            => OnVisiting(data, yieldBreakStatement, VisitingYieldBreakStatement)
            ?? OnVisited(data, yieldBreakStatement, VisitedYieldBreakStatement, TranslateYieldBreakStatement(yieldBreakStatement, data));

        protected virtual IEnumerable<Syntax> TranslateYieldBreakStatement(YieldBreakStatement yieldBreakStatement, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(YieldBreakStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitYieldReturnStatement(YieldReturnStatement yieldReturnStatement, ILTranslationContext data)
            => OnVisiting(data, yieldReturnStatement, VisitingYieldReturnStatement)
            ?? OnVisited(data, yieldReturnStatement, VisitedYieldReturnStatement, TranslateYieldReturnStatement(yieldReturnStatement, data));

        protected virtual IEnumerable<Syntax> TranslateYieldReturnStatement(YieldReturnStatement yieldReturnStatement, ILTranslationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(YieldReturnStatement));
        }

        private static NotImplementedException GetNotImplementedException([CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
            => new NotImplementedException($"{memberName}は実装されていません。{Path.GetFileName(filePath)}@{lineNumber}");
    }
}