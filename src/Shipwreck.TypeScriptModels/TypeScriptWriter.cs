using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Declarations;
using Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels
{
    public class TypeScriptWriter : IExpressionVisitor<int>, IObjectLiteralVisitor<int>
    {
        private readonly IndentedTextWriter _Writer;

        public TypeScriptWriter(TextWriter writer)
        {
            _Writer = (writer as IndentedTextWriter) ?? new IndentedTextWriter(writer);
        }

        // 4.2
        public int VisitThis()
        {
            _Writer.Write("this");
            return 0;
        }

        // 4.3
        public int VisitIdentifier(IdentifierExpression expression)
        {
            _Writer.Write(expression.Name);
            return 0;
        }

        // 4.4
        public int VisitNull()
        {
            _Writer.Write("null");
            return 0;
        }

        public int VisitBoolean(BooleanExpression expression)
        {
            _Writer.Write(expression.Value ? "true" : "false");
            return 0;
        }

        public int VisitNumber(NumberExpression expression)
        {
            //todo:
            _Writer.Write(expression.Value);
            return 0;
        }

        public int VisitString(StringExpression expression)
        {
            _Writer.WriteLiteral(expression.Value);
            return 0;
        }

        public int VisitRegExp(RegExpExpression expression)
        {
            _Writer.Write('/');
            _Writer.Write(expression.Pattern);
            _Writer.Write('/');
            _Writer.Write(expression.Option);
            return 0;
        }

        // 4.5
        public int VisitObject(ObjectExpression expression)
        {
            // TODO: Arrow Function直後の場合はカッコが必要
            _Writer.WriteLine('{');

            if (expression.HasMember)
            {
                _Writer.Indent++;
                for (var i = 0; i < expression.Members.Count; i++)
                {
                    expression.Members[i].Accept(this);
                    if (i < expression.Members.Count - 1)
                    {
                        _Writer.Write(',');
                    }
                    _Writer.WriteLine();
                }
                _Writer.Indent--;
            }
            _Writer.Write('}');

            return 0;
        }

        #region IObjectLiteralVisitor<int>

        public int VisitMemberInitializer(ObjectMemberInitializer member)
        {
            _Writer.Write(member.PropertyName);
            if (member.Value != null)
            {
                _Writer.Write(": ");
                member.Value.Accept(this);
            }
            return 0;
        }

        int IObjectLiteralVisitor<int>.VisitMethod(MethodDeclaration member)
        {
            VisitMethodDeclarationCore(member);
            return 0;
        }

        int IObjectLiteralVisitor<int>.VisitGetAccessor(GetAccessorDeclaration member)
        {
            VisitGetAccessorCore(member);
            return 0;
        }

        int IObjectLiteralVisitor<int>.VisitSetAccessor(SetAccessorDeclaration member)
        {
            VisitSetAccessorCore(member);
            return 0;
        }

        #endregion IObjectLiteralVisitor<int>

        // 4.6
        public int VisitArray(ArrayExpression expression)
        {
            _Writer.WriteLine('[');

            if (expression.HasElement)
            {
                _Writer.Indent++;
                for (var i = 0; i < expression.Elements.Count; i++)
                {
                    expression.Elements[i].Accept(this);
                    if (i < expression.Elements.Count - 1)
                    {
                        _Writer.Write(',');
                    }
                }
                _Writer.Indent--;
            }
            _Writer.Write(']');
            return 0;
        }

        // TODO:4.7

        // 4.8
        public int VisitParentheses(ParenthesesExpression expression)
        {
            _Writer.Write('(');
            expression.Expression.Accept(this);
            _Writer.Write(')');

            return 0;
        }

        // 4.9
        public int VisitSuper()
        {
            _Writer.Write("super");
            return 0;
        }


        // 4.10
        public int VisitFunction(FunctionExpression expression)
        {
            _Writer.Write("function");
            if (!string.IsNullOrEmpty(expression.FunctionName))
            {
                _Writer.Write(' ');
                _Writer.Write(expression.FunctionName);
            }
            _Writer.WriteCallSignature(expression);


            WriteMethodBody();
            return 0;
        }

        // 4.11
        public int VisitArrowFunction(ArrowFunctionExpression expression)
        {
            if (expression.HasParameter)
            {
                _Writer.WriteParameters(expression.Parameters);
            }
            else
            {
                _Writer.Write("()");
            }
            _Writer.Write(" => ");

            // TODO: Statementによって変える
            WriteMethodBody();
            return 0;
        }

        // 4.13
        public int VisitProperty(PropertyExpression property)
        {
            property.Accept(this);
            if (property.IsValidIdentifier)
            {
                _Writer.Write('.');
                _Writer.Write(property.Property);
            }
            else
            {
                _Writer.Write('[');
                _Writer.WriteLiteral(property.Property);
                _Writer.Write(']');
            }
            return 0;
        }

        public int VisitMethod(MethodDeclaration member)
        {
            VisitMethodDeclarationCore(member);
            _Writer.WriteLine();
            return 0;
        }

        private void VisitMethodDeclarationCore(MethodDeclaration member)
        {
            _Writer.WriteAccessibility(member.Accessibility);
            _Writer.Write(member.MethodName);
            _Writer.WriteCallSignature(member);

            WriteMethodBody();
        }

        public int VisitGetAccessor(GetAccessorDeclaration member)
        {
            VisitGetAccessorCore(member);
            _Writer.WriteLine();
            return 0;
        }

        public int VisitSetAccessor(SetAccessorDeclaration member)
        {
            VisitSetAccessorCore(member);
            _Writer.WriteLine();
            return 0;
        }

        private void VisitGetAccessorCore(AccessorDeclaration member)
        {
            _Writer.WriteAccessibility(member.Accessibility);
            _Writer.Write(member.PropertyName);
            _Writer.Write(" get()");
            if (member.PropertyType != null)
            {
                _Writer.Write(": ");
                member.PropertyType.WriteTypeReference(_Writer);
            }

            WriteMethodBody();
        }

        private void VisitSetAccessorCore(SetAccessorDeclaration member)
        {
            _Writer.WriteAccessibility(member.Accessibility);
            _Writer.Write(member.PropertyName);
            _Writer.Write(" set(");
            _Writer.Write(member.ParameterName);
            _Writer.Write(": ");
            member.PropertyType.WriteTypeReference(_Writer);
            _Writer.Write(')');

            WriteMethodBody();
        }

        private void WriteMethodBody()
        {
            _Writer.WriteLine(" {");
            _Writer.Indent++;
            // TODO:
            _Writer.Indent--;
            _Writer.Write('}');

            throw new NotImplementedException();
        }
    }
}