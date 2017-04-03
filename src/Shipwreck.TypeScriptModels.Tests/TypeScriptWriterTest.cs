using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shipwreck.TypeScriptModels.Declarations;
using Shipwreck.TypeScriptModels.Expressions;
using Shipwreck.TypeScriptModels.Statements;

namespace Shipwreck.TypeScriptModels
{
    [TestClass]
    public class TypeScriptWriterTest
    {
        [TestMethod]
        public void VisitThisTest()
            => AssertExpression(new ThisExpression(), "this");

        [TestMethod]
        public void VisitSuperTest()
            => AssertExpression(new SuperExpression(), "super");

        #region VisitVariableDeclarationTest

        [TestMethod]
        public void VisitVariableDeclarationTest_Empty()
            => AssertStatement(new VariableDeclaration(), "");

        [TestMethod]
        public void VisitVariableDeclarationTest_Var()
            => AssertStatement(new VariableDeclaration()
            {
                Bindings = new Collection<VariableBinding>()
                {
                    new VariableBinding() {
                        Variable = new IdentifierExpression() { Name= "hoge" },
                    }
                }
            }, "var hoge;\r\n");

        [TestMethod]
        public void VisitVariableDeclarationTest_VarTyped()
            => AssertStatement(new VariableDeclaration()
            {
                Bindings = new Collection<VariableBinding>()
                {
                    new VariableBinding()
                    {
                        Variable = new IdentifierExpression() { Name= "hoge" },
                        Type = PredefinedType.Any
                    }
                }
            }, "var hoge : any;\r\n");

        [TestMethod]
        public void VisitVariableDeclarationTest_VarTypedInitializer()
            => AssertStatement(new VariableDeclaration()
            {
                Bindings = new Collection<VariableBinding>()
                {
                    new VariableBinding()
                    {
                        Variable = new IdentifierExpression() { Name= "hoge" },
                        Type = PredefinedType.Any,
                        Initializer = new StringExpression() { Value="fuga" }
                    }
                }
            }, "var hoge : any = \"fuga\";\r\n");

        [TestMethod]
        public void VisitVariableDeclarationTest_VarMultiple()
            => AssertStatement(new VariableDeclaration()
            {
                Bindings = new Collection<VariableBinding>()
                {
                    new VariableBinding()
                    {
                        Variable = new IdentifierExpression() { Name= "hoge" },
                    },
                    new VariableBinding()
                    {
                        Variable = new IdentifierExpression() { Name= "piyo" },
                    }
                }
            }, "var hoge, piyo;\r\n");

        [TestMethod]
        public void VisitVariableDeclarationTest_ExportVar()
            => AssertStatement(new VariableDeclaration()
            {
                IsExport = true,
                Bindings = new Collection<VariableBinding>()
                {
                    new VariableBinding() {
                        Variable = new IdentifierExpression() { Name= "hoge" },
                    }
                }
            }, "export var hoge;\r\n");

        [TestMethod]
        public void VisitVariableDeclarationTest_DeclareVar()
            => AssertStatement(new VariableDeclaration()
            {
                IsDeclare = true,
                Bindings = new Collection<VariableBinding>()
                {
                    new VariableBinding() {
                        Variable = new IdentifierExpression() { Name= "hoge" },
                    }
                }
            }, "declare var hoge;\r\n");

        [TestMethod]
        public void VisitVariableDeclarationTest_Let()
            => AssertStatement(new VariableDeclaration()
            {
                Type = VariableDeclarationType.Let,
                Bindings = new Collection<VariableBinding>()
                {
                    new VariableBinding() {
                        Variable = new IdentifierExpression() { Name= "hoge" },
                    }
                }
            }, "let hoge;\r\n");

        [TestMethod]
        public void VisitVariableDeclarationTest_Const()
            => AssertStatement(new VariableDeclaration()
            {
                Type = VariableDeclarationType.Const,
                Bindings = new Collection<VariableBinding>()
                {
                    new VariableBinding() {
                        Variable = new IdentifierExpression() { Name= "hoge" },
                    }
                }
            }, "const hoge;\r\n");

        #endregion VisitVariableDeclarationTest

        private static void AssertExpression(Expression expression, string expected)
        {
            using (var sw = new StringWriter())
            {
                var tsw = new TypeScriptWriter(sw);

                expression.Accept(tsw);

                Assert.AreEqual(expected, sw.ToString());
            }
        }

        private static void AssertStatement(Statement expression, string expected)
        {
            using (var sw = new StringWriter())
            {
                var tsw = new TypeScriptWriter(sw);

                expression.Accept(tsw);

                Assert.AreEqual(expected, sw.ToString());
            }
        }
    }
}