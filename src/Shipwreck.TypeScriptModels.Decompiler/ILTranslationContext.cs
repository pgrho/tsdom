using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Cecil;
using System;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public sealed class ILTranslationContext
    {
        private readonly ILTranslator _Translator;
        private readonly SyntaxTree _SyntaxTree;

        private readonly ILTranslationContext _Root;
        private CSharpUnresolvedFile _UnresolvedFile;
        private ICompilation _Compilation;
        private CSharpAstResolver _AstResolver;

        internal ILTranslationContext(ILTranslator translator, SyntaxTree syntaxTree)
        {
            _Translator = translator;
            _SyntaxTree = syntaxTree;
        }

        internal ILTranslationContext(ILTranslationContext root)
        {
            _Root = root;
        }

        public ILTranslator Translator
            => _Root?._Translator ?? _Translator;

        public SyntaxTree SyntaxTree
            => _Root?._SyntaxTree ?? _SyntaxTree;

        internal CSharpUnresolvedFile UnresolvedFile
        {
            get
            {
                if (_Root != null)
                {
                    return _Root.UnresolvedFile;
                }

                return _UnresolvedFile ?? (_UnresolvedFile = SyntaxTree.ToTypeSystem());
            }
        }

        internal ICompilation Compilation
        {
            get
            {
                if (_Root != null)
                {
                    return _Root.Compilation;
                }
                if (_Compilation == null)
                {
                    Translator.Project.AddOrUpdateFiles(UnresolvedFile);
                    _Compilation = Translator.Project.CreateCompilation();
                }
                return _Compilation;
            }
        }

        public CSharpAstResolver AstResolver
        {
            get
            {
                if (_Root != null)
                {
                    return _Root.AstResolver;
                }

                return _AstResolver ?? (_AstResolver = new CSharpAstResolver(Compilation, SyntaxTree));
            }
        }

        public bool HasYield { get; set; }
        public bool HasAwait { get; set; }

        public ILTranslationContext GetChildContext()
            => new ILTranslationContext(_Root ?? this);

        public Type ResoleClrType(AstNode node)
            => node.Annotation<TypeInformation>()?.InferredType?.ResolveClrType()
                ?? node.Annotation<PropertyDefinition>()?.PropertyType?.ResolveClrType()
                ?? node.Annotation<EventDefinition>()?.EventType?.ResolveClrType()
                ?? node.Annotation<MethodDefinition>()?.ReturnType?.ResolveClrType()
                ?? node.Annotation<FieldDefinition>()?.FieldType?.ResolveClrType();
    }
}