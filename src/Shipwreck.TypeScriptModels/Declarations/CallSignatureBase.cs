﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public abstract class CallSignatureBase : Signature, ICallSignature
    {
        private Collection<TypeParameter> _TypeParameters;

        public bool HasTypeParameter
            => _TypeParameters?.Count > 0;

        public Collection<TypeParameter> TypeParameters
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _TypeParameters);
            }
            set
            {
                CollectionHelper.Set(ref _TypeParameters, value);
            }
        }

        private Collection<Parameter> _Parameters;

        public bool HasParameter
            => _Parameters?.Count > 0;

        public Collection<Parameter> Parameters
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Parameters);
            }
            set
            {
                CollectionHelper.Set(ref _Parameters, value);
            }
        }

        public ITypeReference ReturnType { get; set; }

        internal override void WriteSignature(TextWriter writer) 
            => writer.WriteCallSignature(this);
    }
}