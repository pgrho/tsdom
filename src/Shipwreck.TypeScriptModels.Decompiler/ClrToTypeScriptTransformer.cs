using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;
using Mono.Cecil;
using E = Shipwreck.TypeScriptModels.Expressions;
using D = Shipwreck.TypeScriptModels.Declarations;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public class ClrToTypeScriptTransformer : IAstVisitor<string, IEnumerable<Syntax>>
    {
        private List<D.IModuleDeclaration> _Modules;

        public List<D.IModuleDeclaration> Modules
            => _Modules ?? (_Modules = new List<D.IModuleDeclaration>());

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
            var ms = Modules;
            var m = ms.FirstOrDefault(e => e.Name == fullName);
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

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, string data)
        {
            throw new NotImplementedException();
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

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, string data)
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

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitFieldDeclaration(FieldDeclaration fieldDeclaration, string data)
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

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitIfElseStatement(IfElseStatement ifElseStatement, string data)
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

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitInvocationExpression(InvocationExpression invocationExpression, string data)
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

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitMemberType(MemberType memberType, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitMethodDeclaration(MethodDeclaration methodDeclaration, string data)
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

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, string data)
        {
            throw new NotImplementedException();
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

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitReturnStatement(ReturnStatement returnStatement, string data)
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

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, string data)
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

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitTypeDeclaration(TypeDeclaration typeDeclaration, string data)
        {
            D.ITypeDeclaration td;
            switch (typeDeclaration.ClassType)
            {
                case ClassType.Class:
                    td = new D.ClassDeclaration()
                    {
                        IsAbstract = typeDeclaration.HasModifier(Modifiers.Abstract)
                    };
                    break;

                case ClassType.Struct:
                    td = new D.ClassDeclaration();
                    break;

                case ClassType.Interface:
                    td = new D.InterfaceDeclaration();
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
