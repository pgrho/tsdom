using Shipwreck.TypeScriptModels.Declarations;
using System;
using System.Linq;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations
{
    public sealed class TransformingContext<T>
        where T : class
    {
        private ClassDeclaration _Result;

        internal ClassDeclaration Result
        {
            get
            {
                if (_Result == null)
                {
                    var clr2ts = new ILTranslator();
                    var outs = clr2ts.Transform(typeof(T));

                    foreach (var m in outs)
                    {
                        Console.WriteLine(m);
                    }

                    _Result = outs.OfType<NamespaceDeclaration>().SingleOrDefault()?.Members.OfType<ClassDeclaration>().LastOrDefault(d => d.Name.EndsWith(typeof(T).Name))
                            ?? outs.OfType<ClassDeclaration>().Single(d => d.Name.EndsWith(typeof(T).Name));
                }
                return _Result;
            }
        }

        internal FieldDeclaration GetField(string name)
            => Result.Members.OfType<FieldDeclaration>().SingleOrDefault(m => m.FieldName == name);

        internal ConstructorDeclaration GetConstructor()
            => Result.Members.OfType<ConstructorDeclaration>().SingleOrDefault();

        internal GetAccessorDeclaration GetGetAccessor(string name)
            => Result.Members.OfType<GetAccessorDeclaration>().SingleOrDefault(m => m.PropertyName == name);

        internal SetAccessorDeclaration GetSetAccessor(string name)
            => Result.Members.OfType<SetAccessorDeclaration>().SingleOrDefault(m => m.PropertyName == name);

        internal MethodDeclaration GetMethod(string name)
            => Result.Members.OfType<MethodDeclaration>().SingleOrDefault(m => m.MethodName == name);
    }
}