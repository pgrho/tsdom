using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using D = Shipwreck.TypeScriptModels.Declarations;
using E = Shipwreck.TypeScriptModels.Expressions;
using S = Shipwreck.TypeScriptModels.Statements;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    partial class ILTranslator
    {
        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitFieldDeclaration(FieldDeclaration fieldDeclaration, ILTransformationContext data)
        {
            foreach (var v in fieldDeclaration.Variables)
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

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, ILTransformationContext data)
        {
            var cd = new D.ConstructorDeclaration();

            // TODO: Accessiblity

            foreach (var p in GetParameters(data, constructorDeclaration.Parameters))
            {
                cd.Parameters.Add(p);
            }

            if (!constructorDeclaration.Body.IsNull)
            {
                cd.Statements = GetStatements(constructorDeclaration.Body, data);
            }

            yield return cd;
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitMethodDeclaration(MethodDeclaration methodDeclaration, ILTransformationContext data)
            => OnVisiting(data, methodDeclaration, VisitingMethodDeclaration)
            ?? OnVisited(data, methodDeclaration, VisitedMethodDeclaration, TranslateMethodDeclaration(methodDeclaration, data));

        protected virtual D.MethodDeclaration TranslateMethodDeclaration(MethodDeclaration methodDeclaration, ILTransformationContext data)
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

            if (!methodDeclaration.Body.IsNull)
            {
                var ctx = data.GetChildContext();

                md.Statements = GetStatements(methodDeclaration.Body, ctx);

                md.IsAsync = ctx.HasAwait;
            }

            return md;
        }

        private IEnumerable<D.Parameter> GetParameters(ILTransformationContext data, AstNodeCollection<ParameterDeclaration> parameters)
        {
            foreach (var p in parameters)
            {
                var dp = new D.Parameter()
                {
                    ParameterName = p.Name,
                };
                dp.ParameterType = GetTypeReference(p.Type);

                dp.Decorators = GetDecorators(p.Attributes, data);

                if (p.DefaultExpression?.IsNull == false)
                {
                    dp.Initializer = (Expression)p.DefaultExpression.AcceptVisitor(this, data).LastOrDefault();
                }
                dp.IsOptional = dp.Initializer != null;
                yield return dp;
            }
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, ILTransformationContext data)
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
                        getter.Statements = GetStatements(propertyDeclaration.Getter.Body, data);
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
                        setter.Statements = GetStatements(propertyDeclaration.Setter.Body, data);
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

        #region 対象外

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitTypeParameterDeclaration(TypeParameterDeclaration typeParameterDeclaration, ILTransformationContext data)
        {
            throw new NotSupportedException();
        }

        #endregion 対象外
    }
}