using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels
{
    public class TypeScriptContext
    {
        private TypeScriptCollection<Statement> _Statements;

        public TypeScriptCollection<Statement> Statements
        {
            get
            {
                return _Statements ?? (_Statements = new TypeScriptCollection<Statement>());
            }
            set
            {
                _Statements?.Clear();
                if (value?.Count > 0)
                {
                    Statements.AddRange(value);
                }
            }
        }

        public ModuleDeclaration GetModule(string name)
            => _Statements?.OfType<ModuleDeclaration>().FirstOrDefault(_ => _.Name == name);

        public FlagedType ResolveType(string name, Func<string, FlagedType> resolver = null)
        {
            resolver = resolver ?? (n => ResolveType(n));
            if (name.EndsWith(")"))
            {
                if (name == "function()")
                {
                    return new FlagedType(new FunctionType(new FlagedType[0]));
                }
                string core;
                bool fn;
                if (name.StartsWith("function("))
                {
                    core = name.Substring(9, name.Length - 10);
                    fn = true;
                }
                else if (name.StartsWith("("))
                {
                    core = name.Substring(1, name.Length - 2);
                    fn = false;
                }
                else
                {
                    core = null;
                    fn = false;
                }
                if (core != null)
                {
                    var types = ParseFlagedTypeList(core, resolver);

                    if (fn)
                    {
                        return new FlagedType(new FunctionType(types));
                    }
                    else
                    {
                        return CreateUnionType(types);
                    }
                }
            }

            if (name.StartsWith("{") && name.EndsWith("}"))
            {
                var n = 0;
                var s = 1;

                string pn = null;
                var ot = new ObjectType();
                for (var i = 1; i < name.Length - 1; i++)
                {
                    if (name[i] == '(' || name[i] == '{')
                    {
                        n++;
                    }
                    else if (name[i] == ')' || name[i] == '}')
                    {
                        n--;

                        if (n == 0)
                        {
                            if (pn != null)
                            {
                                var tn = name.Substring(s, i - s).Trim();
                                var pt = resolver(tn);
                                ot.Properties.Add(new ObjectTypeProperty()
                                {
                                    Name = pn,
                                    PropertyType = pt.Type,
                                    IsRequired = !pt.IsNullable
                                });
                                s = i + 1;
                                pn = null;
                            }
                        }
                    }
                    else if (n == 0)
                    {
                        if (name[i] == ':')
                        {
                            pn = name.Substring(s, i - s).Trim();
                            s = i + 1;
                        }
                        else if (name[i] == ',')
                        {
                            var tn = name.Substring(s, i - s).Trim();
                            var pt = resolver(tn);
                            ot.Properties.Add(new ObjectTypeProperty()
                            {
                                Name = pn,
                                PropertyType = pt.Type,
                                IsRequired = !pt.IsNullable
                            });
                            s = i + 1;
                        }
                    }
                }

                return new FlagedType(ot);
            }

            if (name.StartsWith("non-null "))
            {
                var bt = resolver(name.Substring(9));
                return new FlagedType(bt.Type, TypeFlags.Required);
            }

            if (name.StartsWith("nullable "))
            {
                var bt = resolver(name.Substring(9));
                return new FlagedType(bt.Type, TypeFlags.Nullable);
            }

            if (name.StartsWith("Array of "))
            {
                var et = resolver(name.Substring(9));
                return new FlagedType(new ArrayType(et.Type));
            }
            if (name.StartsWith("Promise containing "))
            {
                var et = resolver(name.Substring(19));
                return new FlagedType(new PromiseType(et.Type));
            }

            if ("any".Equals(name, System.StringComparison.InvariantCultureIgnoreCase)
                || "any type".Equals(name, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return new FlagedType(BuiltinType.Any);
            }
            if ("number".Equals(name, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return new FlagedType(BuiltinType.Number);
            }
            if ("boolean".Equals(name, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return new FlagedType(BuiltinType.Boolean);
            }
            if ("string".Equals(name, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return new FlagedType(BuiltinType.String);
            }
            if ("function".Equals(name, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return new FlagedType(BuiltinType.Function);
            }
            if (name == "Error")
            {
                return new FlagedType(BuiltinType.Error);
            }
            if (name == "Object")
            {
                return new FlagedType(BuiltinType.Object);
            }
            var t = FindType(name);

            //if (t == null)
            //{
            //    if (_UnknownTypes == null)
            //    {
            //        _UnknownTypes = new Dictionary<string, UnknownType>();
            //    }
            //    UnknownType ut;
            //    if (!_UnknownTypes.TryGetValue(name, out ut))
            //    {
            //        ut = new UnknownType(name);
            //        _UnknownTypes[name] = ut;
            //    }
            //    return new FlagedType(ut);
            //}

            return new FlagedType(t);
        }

        private static FlagedType CreateUnionType(List<FlagedType> types)
        {
            if (types.Count == 0)
            {
                return new FlagedType(BuiltinType.Any);
            }
            else if (types.Count == 1)
            {
                return types[0];
            }

            if (types.All(_ => _.Type is FunctionType))
            {
                return new FlagedType(new FunctionType(Enumerable.Range(0, ((FunctionType)types[0].Type).ParameterTypes.Length).Select(i =>
                {
                    var pts = types.Select(t => ((FunctionType)t.Type).ParameterTypes.ElementAtOrDefault(i)).ToList();

                    var ft = pts.Select(_ => _.Type).FirstOrDefault(_ => _ != null);

                    if (ft == null)
                    {
                        return new FlagedType(BuiltinType.Any, TypeFlags.Nullable);
                    }
                    else if (pts.Any(_ => _.Type != null && _.Type != ft))
                    {
                        return new FlagedType(new UnionType(pts.Select(_ => _.Type).Where(_ => _ != null)));
                    }
                    else
                    {
                        return new FlagedType(ft, pts.Any(_ => _.Type == null || _.IsNullable) ? TypeFlags.Nullable : TypeFlags.None);
                    }
                })));
            }

            return new FlagedType(new UnionType(types.Select(_ => _.Type)));
        }

        public ITypeScriptType FindType(string name)
        {
            return _Statements?.OfType<ITypeScriptType>().FirstOrDefault(_ => _.Name == name)
                    ?? _Statements?.OfType<ModuleDeclaration>().SelectMany(
                            m => m.Statements
                                    .OfType<ITypeScriptType>()
                                    .Where(t2 => name.Length == m.Name.Length + t2.Name.Length + 1
                                                && name.StartsWith(m.Name)
                                                && name[m.Name.Length] == '.'
                                                && name.EndsWith(t2.Name))).FirstOrDefault();
        }

        private List<FlagedType> ParseFlagedTypeList(string core, Func<string, FlagedType> resolver = null)
        {
            var n = 0;
            var s = 0;
            var types = new List<FlagedType>();
            for (var i = 0; i < core.Length; i++)
            {
                if (core[i] == '(')
                {
                    n++;
                }
                else if (core[i] == ')')
                {
                    n--;
                }
                else if (n == 0)
                {
                    if (core[i] == ',')
                    {
                        var tn = core.Substring(s, i - s).Trim();
                        types.Add(resolver(tn));
                        s = i + 1;
                    }
                    else if (i > 2 && core[i - 3] == ' ' && core[i - 2] == 'o' && core[i - 1] == 'r' && core[i] == ' ')
                    {
                        var tn = core.Substring(s, i - s - 3).Trim();
                        if (!string.IsNullOrWhiteSpace(tn))
                        {
                            types.Add(resolver(tn));
                        }
                        s = i + 1;
                    }
                }
            }
            {
                var tn = core.Substring(s).Trim();
                types.Add(resolver(tn));
            }
            return types;
        }

        public void WriteAsDeclaration(TextWriter writer)
        {
            var iw = writer as IndentedTextWriter;
            if (iw == null)
            {
                using (var sw = new StringWriter())
                using (var itw = new IndentedTextWriter(sw))
                {
                    WriteAsDeclaration(itw);
                    itw.Flush();
                    writer.Write(sw);
                }
            }
            else
            {
                WriteAsDeclaration(iw);
            }
        }

        public void WriteAsDeclaration(IndentedTextWriter writer)
        {
            if (_Statements != null)
            {
                foreach (var s in _Statements)
                {
                    s.WriteAsDeclaration(writer);
                }
            }
        }
    }
}