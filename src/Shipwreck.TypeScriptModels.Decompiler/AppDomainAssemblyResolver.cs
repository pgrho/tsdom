using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    internal sealed class AppDomainAssemblyResolver : IAssemblyResolver, IDisposable
    {
        private List<FileStream> _Files;

        public void Dispose()
        {
            if (_Files != null)
            {
                foreach (var s in _Files)
                {
                    try
                    {
                        s.Dispose();
                    }
                    catch
                    {

                    }
                }
                _Files = null;
            }
        }

        public AssemblyDefinition Resolve(string fullName)
            => Resolve(fullName, new ReaderParameters());

        public AssemblyDefinition Resolve(AssemblyNameReference name)
            => Resolve(name, new ReaderParameters());

        public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
        {
            var a = AppDomain.CurrentDomain.GetAssemblies().First(e => e.FullName.Equals(fullName, StringComparison.InvariantCultureIgnoreCase));
            return GetDefinition(a, parameters);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            var an = new AssemblyName(name.FullName);
            var a = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(e => e.GetName().Name.Equals(an.Name, StringComparison.InvariantCultureIgnoreCase));
            if (a == null)
            {
                return null;
            }
            return GetDefinition(a, parameters);
        }


        private AssemblyDefinition GetDefinition(Assembly a, ReaderParameters parameters)
        {
            var fs = new FileStream(a.Location, FileMode.Open, FileAccess.Read);
            (_Files ?? (_Files = new List<FileStream>())).Add(fs);

            return AssemblyDefinition.ReadAssembly(fs, parameters);
        }
    }
}