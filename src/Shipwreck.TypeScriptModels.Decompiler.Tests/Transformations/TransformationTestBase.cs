using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shipwreck.TypeScriptModels.Declarations;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations
{
    public abstract class TransformationTestBase<T>
        where T : class
    {
        private ClassDeclaration _Result;

        protected ClassDeclaration Result
        {
            get
            {
                if (_Result == null)
                {
                    var clr2ts = new ILTranslator();
                    clr2ts.Transform(typeof(T));

                    foreach (var m in clr2ts.Statements)
                    {
                        Console.WriteLine(m);
                    }

                    _Result = clr2ts.Statements.Cast<NamespaceDeclaration>().Single().Members.OfType<ClassDeclaration>().Single();

                    Assert.AreEqual(typeof(T).Name, _Result.Name);
                }
                return _Result;
            }
        }

        protected FieldDeclaration GetField(string name)
            => Result.Members.OfType<FieldDeclaration>().SingleOrDefault(m => m.FieldName == name);

        protected ConstructorDeclaration GetConstructor()
            => Result.Members.OfType<ConstructorDeclaration>().SingleOrDefault();

        protected GetAccessorDeclaration GetGetAccessor(string name)
            => Result.Members.OfType<GetAccessorDeclaration>().SingleOrDefault(m => m.PropertyName == name);

        protected SetAccessorDeclaration GetSetAccessor(string name)
            => Result.Members.OfType<SetAccessorDeclaration>().SingleOrDefault(m => m.PropertyName == name);

        protected MethodDeclaration GetMethod(string name)
            => Result.Members.OfType<MethodDeclaration>().SingleOrDefault(m => m.MethodName == name);
    }
}