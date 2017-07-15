using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using D = Shipwreck.TypeScriptModels.Declarations;

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
                    _DefaultConventions = new ILTranslationConvention[]
                    {
                        new MethodNameConvention(typeof(object).GetMethod(nameof(ToString)), "toString"),
                        new EventConvention(),
                        new MathConventionSet()
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

                return b.SyntaxTree.AcceptVisitor(this, new ILTransformationContext(this, b.SyntaxTree)).ToArray();
            }
        }

        private IEnumerable<Syntax> OnVisiting<T>(ILTransformationContext data, T node, EventHandler<VisitingEventArgs<T>> handler)
            where T : AstNode
        {
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

        private IEnumerable<Syntax> OnVisited<T>(ILTransformationContext data, T node, EventHandler<VisitedEventArgs<T>> handler, Syntax result)
            where T : AstNode
            => OnVisited(data, node, handler, new[] { result });

        private IEnumerable<Syntax> OnVisited<T>(ILTransformationContext data, T node, EventHandler<VisitedEventArgs<T>> handler, IEnumerable<Syntax> results)
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

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitAccessor(Accessor accessor, ILTransformationContext data)
            => OnVisiting(data, accessor, VisitingAccessor)
            ?? OnVisited(data, accessor, VisitedAccessor, TranslateAccessor(accessor, data));

        protected virtual IEnumerable<Syntax> TranslateAccessor(Accessor accessor, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(Accessor));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, ILTransformationContext data)
            => OnVisiting(data, anonymousMethodExpression, VisitingAnonymousMethodExpression)
            ?? OnVisited(data, anonymousMethodExpression, VisitedAnonymousMethodExpression, TranslateAnonymousMethodExpression(anonymousMethodExpression, data));

        protected virtual IEnumerable<Syntax> TranslateAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(AnonymousMethodExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitAnonymousTypeCreateExpression(AnonymousTypeCreateExpression anonymousTypeCreateExpression, ILTransformationContext data)
            => OnVisiting(data, anonymousTypeCreateExpression, VisitingAnonymousTypeCreateExpression)
            ?? OnVisited(data, anonymousTypeCreateExpression, VisitedAnonymousTypeCreateExpression, TranslateAnonymousTypeCreateExpression(anonymousTypeCreateExpression, data));

        protected virtual IEnumerable<Syntax> TranslateAnonymousTypeCreateExpression(AnonymousTypeCreateExpression anonymousTypeCreateExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(AnonymousTypeCreateExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, ILTransformationContext data)
            => OnVisiting(data, arrayCreateExpression, VisitingArrayCreateExpression)
            ?? OnVisited(data, arrayCreateExpression, VisitedArrayCreateExpression, TranslateArrayCreateExpression(arrayCreateExpression, data));

        protected virtual IEnumerable<Syntax> TranslateArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ArrayCreateExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, ILTransformationContext data)
            => OnVisiting(data, arrayInitializerExpression, VisitingArrayInitializerExpression)
            ?? OnVisited(data, arrayInitializerExpression, VisitedArrayInitializerExpression, TranslateArrayInitializerExpression(arrayInitializerExpression, data));

        protected virtual IEnumerable<Syntax> TranslateArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ArrayInitializerExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitArraySpecifier(ArraySpecifier arraySpecifier, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ArraySpecifier));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitAsExpression(AsExpression asExpression, ILTransformationContext data)
            => OnVisiting(data, asExpression, VisitingAsExpression)
            ?? OnVisited(data, asExpression, VisitedAsExpression, TranslateAsExpression(asExpression, data));

        protected virtual IEnumerable<Syntax> TranslateAsExpression(AsExpression asExpression, ILTransformationContext data)
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

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitCaseLabel(CaseLabel caseLabel, ILTransformationContext data)
            => OnVisiting(data, caseLabel, VisitingCaseLabel)
            ?? OnVisited(data, caseLabel, VisitedCaseLabel, TranslateCaseLabel(caseLabel, data));

        protected virtual IEnumerable<Syntax> TranslateCaseLabel(CaseLabel caseLabel, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(CaseLabel));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitCastExpression(CastExpression castExpression, ILTransformationContext data)
            => OnVisiting(data, castExpression, VisitingCastExpression)
            ?? OnVisited(data, castExpression, VisitedCastExpression, TranslateCastExpression(castExpression, data));

        protected virtual IEnumerable<Syntax> TranslateCastExpression(CastExpression castExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(CastExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitCatchClause(CatchClause catchClause, ILTransformationContext data)
            => OnVisiting(data, catchClause, VisitingCatchClause)
            ?? OnVisited(data, catchClause, VisitedCatchClause, TranslateCatchClause(catchClause, data));

        protected virtual IEnumerable<Syntax> TranslateCatchClause(CatchClause catchClause, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(CatchClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitCheckedExpression(CheckedExpression checkedExpression, ILTransformationContext data)
            => OnVisiting(data, checkedExpression, VisitingCheckedExpression)
            ?? OnVisited(data, checkedExpression, VisitedCheckedExpression, TranslateCheckedExpression(checkedExpression, data));

        protected virtual IEnumerable<Syntax> TranslateCheckedExpression(CheckedExpression checkedExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(CheckedExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitCheckedStatement(CheckedStatement checkedStatement, ILTransformationContext data)
            => OnVisiting(data, checkedStatement, VisitingCheckedStatement)
            ?? OnVisited(data, checkedStatement, VisitedCheckedStatement, TranslateCheckedStatement(checkedStatement, data));

        protected virtual IEnumerable<Syntax> TranslateCheckedStatement(CheckedStatement checkedStatement, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(CheckedStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitComment(Comment comment, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(Comment));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitComposedType(ComposedType composedType, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ComposedType));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitConditionalExpression(ConditionalExpression conditionalExpression, ILTransformationContext data)
            => OnVisiting(data, conditionalExpression, VisitingConditionalExpression)
            ?? OnVisited(data, conditionalExpression, VisitedConditionalExpression, TranslateConditionalExpression(conditionalExpression, data));

        protected virtual IEnumerable<Syntax> TranslateConditionalExpression(ConditionalExpression conditionalExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ConditionalExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitConstraint(Constraint constraint, ILTransformationContext data)
            => OnVisiting(data, constraint, VisitingConstraint)
            ?? OnVisited(data, constraint, VisitedConstraint, TranslateConstraint(constraint, data));

        protected virtual IEnumerable<Syntax> TranslateConstraint(Constraint constraint, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(Constraint));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitConstructorInitializer(ConstructorInitializer constructorInitializer, ILTransformationContext data)
            => OnVisiting(data, constructorInitializer, VisitingConstructorInitializer)
            ?? OnVisited(data, constructorInitializer, VisitedConstructorInitializer, TranslateConstructorInitializer(constructorInitializer, data));

        protected virtual IEnumerable<Syntax> TranslateConstructorInitializer(ConstructorInitializer constructorInitializer, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ConstructorInitializer));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitCSharpTokenNode(CSharpTokenNode cSharpTokenNode, ILTransformationContext data)
            => OnVisiting(data, cSharpTokenNode, VisitingCSharpTokenNode)
            ?? OnVisited(data, cSharpTokenNode, VisitedCSharpTokenNode, TranslateCSharpTokenNode(cSharpTokenNode, data));

        protected virtual IEnumerable<Syntax> TranslateCSharpTokenNode(CSharpTokenNode cSharpTokenNode, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(CSharpTokenNode));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitCustomEventDeclaration(CustomEventDeclaration customEventDeclaration, ILTransformationContext data)
            => OnVisiting(data, customEventDeclaration, VisitingCustomEventDeclaration)
            ?? OnVisited(data, customEventDeclaration, VisitedCustomEventDeclaration, TranslateCustomEventDeclaration(customEventDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateCustomEventDeclaration(CustomEventDeclaration customEventDeclaration, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(CustomEventDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, ILTransformationContext data)
            => OnVisiting(data, defaultValueExpression, VisitingDefaultValueExpression)
            ?? OnVisited(data, defaultValueExpression, VisitedDefaultValueExpression, TranslateDefaultValueExpression(defaultValueExpression, data));

        protected virtual IEnumerable<Syntax> TranslateDefaultValueExpression(DefaultValueExpression defaultValueExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(DefaultValueExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, ILTransformationContext data)
            => OnVisiting(data, delegateDeclaration, VisitingDelegateDeclaration)
            ?? OnVisited(data, delegateDeclaration, VisitedDelegateDeclaration, TranslateDelegateDeclaration(delegateDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateDelegateDeclaration(DelegateDeclaration delegateDeclaration, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(DelegateDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, ILTransformationContext data)
            => OnVisiting(data, destructorDeclaration, VisitingDestructorDeclaration)
            ?? OnVisited(data, destructorDeclaration, VisitedDestructorDeclaration, TranslateDestructorDeclaration(destructorDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateDestructorDeclaration(DestructorDeclaration destructorDeclaration, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(DestructorDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitDirectionExpression(DirectionExpression directionExpression, ILTransformationContext data)
            => OnVisiting(data, directionExpression, VisitingDirectionExpression)
            ?? OnVisited(data, directionExpression, VisitedDirectionExpression, TranslateDirectionExpression(directionExpression, data));

        protected virtual IEnumerable<Syntax> TranslateDirectionExpression(DirectionExpression directionExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(DirectionExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitDocumentationReference(DocumentationReference documentationReference, ILTransformationContext data)
            => OnVisiting(data, documentationReference, VisitingDocumentationReference)
            ?? OnVisited(data, documentationReference, VisitedDocumentationReference, TranslateDocumentationReference(documentationReference, data));

        protected virtual IEnumerable<Syntax> TranslateDocumentationReference(DocumentationReference documentationReference, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(DocumentationReference));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration, ILTransformationContext data)
            => OnVisiting(data, enumMemberDeclaration, VisitingEnumMemberDeclaration)
            ?? OnVisited(data, enumMemberDeclaration, VisitedEnumMemberDeclaration, TranslateEnumMemberDeclaration(enumMemberDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(EnumMemberDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitErrorNode(AstNode errorNode, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitExternAliasDeclaration(ExternAliasDeclaration externAliasDeclaration, ILTransformationContext data)
            => OnVisiting(data, externAliasDeclaration, VisitingExternAliasDeclaration)
            ?? OnVisited(data, externAliasDeclaration, VisitedExternAliasDeclaration, TranslateExternAliasDeclaration(externAliasDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateExternAliasDeclaration(ExternAliasDeclaration externAliasDeclaration, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ExternAliasDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration, ILTransformationContext data)
            => OnVisiting(data, fixedFieldDeclaration, VisitingFixedFieldDeclaration)
            ?? OnVisited(data, fixedFieldDeclaration, VisitedFixedFieldDeclaration, TranslateFixedFieldDeclaration(fixedFieldDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(FixedFieldDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitFixedStatement(FixedStatement fixedStatement, ILTransformationContext data)
            => OnVisiting(data, fixedStatement, VisitingFixedStatement)
            ?? OnVisited(data, fixedStatement, VisitedFixedStatement, TranslateFixedStatement(fixedStatement, data));

        protected virtual IEnumerable<Syntax> TranslateFixedStatement(FixedStatement fixedStatement, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(FixedStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitFixedVariableInitializer(FixedVariableInitializer fixedVariableInitializer, ILTransformationContext data)
            => OnVisiting(data, fixedVariableInitializer, VisitingFixedVariableInitializer)
            ?? OnVisited(data, fixedVariableInitializer, VisitedFixedVariableInitializer, TranslateFixedVariableInitializer(fixedVariableInitializer, data));

        protected virtual IEnumerable<Syntax> TranslateFixedVariableInitializer(FixedVariableInitializer fixedVariableInitializer, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(FixedVariableInitializer));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, ILTransformationContext data)
            => OnVisiting(data, gotoCaseStatement, VisitingGotoCaseStatement)
            ?? OnVisited(data, gotoCaseStatement, VisitedGotoCaseStatement, TranslateGotoCaseStatement(gotoCaseStatement, data));

        protected virtual IEnumerable<Syntax> TranslateGotoCaseStatement(GotoCaseStatement gotoCaseStatement, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(GotoCaseStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitGotoDefaultStatement(GotoDefaultStatement gotoDefaultStatement, ILTransformationContext data)
            => OnVisiting(data, gotoDefaultStatement, VisitingGotoDefaultStatement)
            ?? OnVisited(data, gotoDefaultStatement, VisitedGotoDefaultStatement, TranslateGotoDefaultStatement(gotoDefaultStatement, data));

        protected virtual IEnumerable<Syntax> TranslateGotoDefaultStatement(GotoDefaultStatement gotoDefaultStatement, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(GotoDefaultStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitGotoStatement(GotoStatement gotoStatement, ILTransformationContext data)
            => OnVisiting(data, gotoStatement, VisitingGotoStatement)
            ?? OnVisited(data, gotoStatement, VisitedGotoStatement, TranslateGotoStatement(gotoStatement, data));

        protected virtual IEnumerable<Syntax> TranslateGotoStatement(GotoStatement gotoStatement, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(GotoStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitIdentifier(Identifier identifier, ILTransformationContext data)
            => OnVisiting(data, identifier, VisitingIdentifier)
            ?? OnVisited(data, identifier, VisitedIdentifier, TranslateIdentifier(identifier, data));

        protected virtual IEnumerable<Syntax> TranslateIdentifier(Identifier identifier, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(Identifier));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, ILTransformationContext data)
            => OnVisiting(data, indexerDeclaration, VisitingIndexerDeclaration)
            ?? OnVisited(data, indexerDeclaration, VisitedIndexerDeclaration, TranslateIndexerDeclaration(indexerDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateIndexerDeclaration(IndexerDeclaration indexerDeclaration, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(IndexerDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitIsExpression(IsExpression isExpression, ILTransformationContext data)
            => OnVisiting(data, isExpression, VisitingIsExpression)
            ?? OnVisited(data, isExpression, VisitedIsExpression, TranslateIsExpression(isExpression, data));

        protected virtual IEnumerable<Syntax> TranslateIsExpression(IsExpression isExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(IsExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitLabelStatement(LabelStatement labelStatement, ILTransformationContext data)
            => OnVisiting(data, labelStatement, VisitingLabelStatement)
            ?? OnVisited(data, labelStatement, VisitedLabelStatement, TranslateLabelStatement(labelStatement, data));

        protected virtual IEnumerable<Syntax> TranslateLabelStatement(LabelStatement labelStatement, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(LabelStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitLambdaExpression(LambdaExpression lambdaExpression, ILTransformationContext data)
            => OnVisiting(data, lambdaExpression, VisitingLambdaExpression)
            ?? OnVisited(data, lambdaExpression, VisitedLambdaExpression, TranslateLambdaExpression(lambdaExpression, data));

        protected virtual IEnumerable<Syntax> TranslateLambdaExpression(LambdaExpression lambdaExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(LambdaExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitMemberType(MemberType memberType, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(MemberType));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, ILTransformationContext data)
            => OnVisiting(data, namedArgumentExpression, VisitingNamedArgumentExpression)
            ?? OnVisited(data, namedArgumentExpression, VisitedNamedArgumentExpression, TranslateNamedArgumentExpression(namedArgumentExpression, data));

        protected virtual IEnumerable<Syntax> TranslateNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(NamedArgumentExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitNamedExpression(NamedExpression namedExpression, ILTransformationContext data)
            => OnVisiting(data, namedExpression, VisitingNamedExpression)
            ?? OnVisited(data, namedExpression, VisitedNamedExpression, TranslateNamedExpression(namedExpression, data));

        protected virtual IEnumerable<Syntax> TranslateNamedExpression(NamedExpression namedExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(NamedExpression));
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
            => OnVisiting(data, operatorDeclaration, VisitingOperatorDeclaration)
            ?? OnVisited(data, operatorDeclaration, VisitedOperatorDeclaration, TranslateOperatorDeclaration(operatorDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateOperatorDeclaration(OperatorDeclaration operatorDeclaration, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(OperatorDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitParameterDeclaration(ParameterDeclaration parameterDeclaration, ILTransformationContext data)
            => OnVisiting(data, parameterDeclaration, VisitingParameterDeclaration)
            ?? OnVisited(data, parameterDeclaration, VisitedParameterDeclaration, TranslateParameterDeclaration(parameterDeclaration, data));

        protected virtual IEnumerable<Syntax> TranslateParameterDeclaration(ParameterDeclaration parameterDeclaration, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ParameterDeclaration));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitPatternPlaceholder(AstNode placeholder, Pattern pattern, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, ILTransformationContext data)
            => OnVisiting(data, pointerReferenceExpression, VisitingPointerReferenceExpression)
            ?? OnVisited(data, pointerReferenceExpression, VisitedPointerReferenceExpression, TranslatePointerReferenceExpression(pointerReferenceExpression, data));

        protected virtual IEnumerable<Syntax> TranslatePointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(PointerReferenceExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitPrimitiveType(PrimitiveType primitiveType, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(PrimitiveType));
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

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryContinuationClause(QueryContinuationClause queryContinuationClause, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryContinuationClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryExpression(QueryExpression queryExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryFromClause(QueryFromClause queryFromClause, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryFromClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryGroupClause(QueryGroupClause queryGroupClause, ILTransformationContext data)
            => OnVisiting(data, queryGroupClause, VisitingQueryGroupClause)
            ?? OnVisited(data, queryGroupClause, VisitedQueryGroupClause, TranslateQueryGroupClause(queryGroupClause, data));

        protected virtual IEnumerable<Syntax> TranslateQueryGroupClause(QueryGroupClause queryGroupClause, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryGroupClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryJoinClause(QueryJoinClause queryJoinClause, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryJoinClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryLetClause(QueryLetClause queryLetClause, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryLetClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryOrderClause(QueryOrderClause queryOrderClause, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryOrderClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryOrdering(QueryOrdering queryOrdering, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryOrdering));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQuerySelectClause(QuerySelectClause querySelectClause, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QuerySelectClause));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitQueryWhereClause(QueryWhereClause queryWhereClause, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(QueryWhereClause));
        }

        #endregion クエリー

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitSimpleType(SimpleType simpleType, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(SimpleType));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitSizeOfExpression(SizeOfExpression sizeOfExpression, ILTransformationContext data)
            => OnVisiting(data, sizeOfExpression, VisitingSizeOfExpression)
            ?? OnVisited(data, sizeOfExpression, VisitedSizeOfExpression, TranslateSizeOfExpression(sizeOfExpression, data));

        protected virtual IEnumerable<Syntax> TranslateSizeOfExpression(SizeOfExpression sizeOfExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(SizeOfExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitStackAllocExpression(StackAllocExpression stackAllocExpression, ILTransformationContext data)
            => OnVisiting(data, stackAllocExpression, VisitingStackAllocExpression)
            ?? OnVisited(data, stackAllocExpression, VisitedStackAllocExpression, TranslateStackAllocExpression(stackAllocExpression, data));

        protected virtual IEnumerable<Syntax> TranslateStackAllocExpression(StackAllocExpression stackAllocExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(StackAllocExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitSwitchSection(SwitchSection switchSection, ILTransformationContext data)
            => OnVisiting(data, switchSection, VisitingSwitchSection)
            ?? OnVisited(data, switchSection, VisitedSwitchSection, TranslateSwitchSection(switchSection, data));

        protected virtual IEnumerable<Syntax> TranslateSwitchSection(SwitchSection switchSection, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(SwitchSection));
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
            => OnVisiting(data, throwStatement, VisitingThrowStatement)
            ?? OnVisited(data, throwStatement, VisitedThrowStatement, TranslateThrowStatement(throwStatement, data));

        protected virtual IEnumerable<Syntax> TranslateThrowStatement(ThrowStatement throwStatement, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(ThrowStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitTypeOfExpression(TypeOfExpression typeOfExpression, ILTransformationContext data)
            => OnVisiting(data, typeOfExpression, VisitingTypeOfExpression)
            ?? OnVisited(data, typeOfExpression, VisitedTypeOfExpression, TranslateTypeOfExpression(typeOfExpression, data));

        protected virtual IEnumerable<Syntax> TranslateTypeOfExpression(TypeOfExpression typeOfExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(TypeOfExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitUncheckedExpression(UncheckedExpression uncheckedExpression, ILTransformationContext data)
            => OnVisiting(data, uncheckedExpression, VisitingUncheckedExpression)
            ?? OnVisited(data, uncheckedExpression, VisitedUncheckedExpression, TranslateUncheckedExpression(uncheckedExpression, data));

        protected virtual IEnumerable<Syntax> TranslateUncheckedExpression(UncheckedExpression uncheckedExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(UncheckedExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitUncheckedStatement(UncheckedStatement uncheckedStatement, ILTransformationContext data)
            => OnVisiting(data, uncheckedStatement, VisitingUncheckedStatement)
            ?? OnVisited(data, uncheckedStatement, VisitedUncheckedStatement, TranslateUncheckedStatement(uncheckedStatement, data));

        protected virtual IEnumerable<Syntax> TranslateUncheckedStatement(UncheckedStatement uncheckedStatement, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(UncheckedStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitUndocumentedExpression(UndocumentedExpression undocumentedExpression, ILTransformationContext data)
            => OnVisiting(data, undocumentedExpression, VisitingUndocumentedExpression)
            ?? OnVisited(data, undocumentedExpression, VisitedUndocumentedExpression, TranslateUndocumentedExpression(undocumentedExpression, data));

        protected virtual IEnumerable<Syntax> TranslateUndocumentedExpression(UndocumentedExpression undocumentedExpression, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(UndocumentedExpression));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitUnsafeStatement(UnsafeStatement unsafeStatement, ILTransformationContext data)
            => OnVisiting(data, unsafeStatement, VisitingUnsafeStatement)
            ?? OnVisited(data, unsafeStatement, VisitedUnsafeStatement, TranslateUnsafeStatement(unsafeStatement, data));

        protected virtual IEnumerable<Syntax> TranslateUnsafeStatement(UnsafeStatement unsafeStatement, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(UnsafeStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitVariableInitializer(VariableInitializer variableInitializer, ILTransformationContext data)
            => OnVisiting(data, variableInitializer, VisitingVariableInitializer)
            ?? OnVisited(data, variableInitializer, VisitedVariableInitializer, TranslateVariableInitializer(variableInitializer, data));

        protected virtual IEnumerable<Syntax> TranslateVariableInitializer(VariableInitializer variableInitializer, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(VariableInitializer));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitWhitespace(WhitespaceNode whitespaceNode, ILTransformationContext data)
        {
            throw GetNotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitYieldBreakStatement(YieldBreakStatement yieldBreakStatement, ILTransformationContext data)
            => OnVisiting(data, yieldBreakStatement, VisitingYieldBreakStatement)
            ?? OnVisited(data, yieldBreakStatement, VisitedYieldBreakStatement, TranslateYieldBreakStatement(yieldBreakStatement, data));

        protected virtual IEnumerable<Syntax> TranslateYieldBreakStatement(YieldBreakStatement yieldBreakStatement, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(YieldBreakStatement));
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitYieldReturnStatement(YieldReturnStatement yieldReturnStatement, ILTransformationContext data)
            => OnVisiting(data, yieldReturnStatement, VisitingYieldReturnStatement)
            ?? OnVisited(data, yieldReturnStatement, VisitedYieldReturnStatement, TranslateYieldReturnStatement(yieldReturnStatement, data));

        protected virtual IEnumerable<Syntax> TranslateYieldReturnStatement(YieldReturnStatement yieldReturnStatement, ILTransformationContext data)
        {
            throw ExceptionHelper.CannotTranslateAst(nameof(YieldReturnStatement));
        }

        private static NotImplementedException GetNotImplementedException([CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
            => new NotImplementedException($"{memberName}は実装されていません。{Path.GetFileName(filePath)}@{lineNumber}");
    }
}