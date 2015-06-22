using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cs2ts
{
    public class Visitor : CSharpSyntaxWalker
    {
        private readonly IList<string> _output;

        private int _indent;

        public Visitor() : base(SyntaxWalkerDepth.Node)
        {
            this._output = new List<string>();
            this._indent = 0;
        }

        private void AddClassScope(ClassDeclarationSyntax node)
        {
            string modifier = Visitor.GetVisibilityModifier(node.Modifiers);

            this.Emit(string.Join(" ", new[] { modifier, "class", node.Identifier.Text }));

            using (IndentScope())
            {
                base.VisitClassDeclaration(node);
            }
        }

        private void Emit(string text, params object[] args)
        {
            this._output.Add(string.Format(string.Concat(new string(' ', this._indent * 4), text), args));
        }

        private string GetMappedType(TypeSyntax type)
        {
            return type.ToString().StartsWith("int") ? "number" : "string";
        }

        private static string GetVisibilityModifier(SyntaxTokenList tokens)
        {
            return tokens.OfType<SyntaxToken>().Any(m => m.Kind() == SyntaxKind.PublicKeyword) ? "public" : "private";
        }

        private Visitor.EndBlock IndentScope()
        {
            return new EndBlock(this);
        }

        public string Output()
        {
            return string.Join(Environment.NewLine, this._output);
        }

        public override void VisitBlock(BlockSyntax node)
        {
            foreach (var statement in node.Statements)
            {
                this.Emit(statement.ToString());
            }
            base.VisitBlock(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            AddClassScope(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            string visibility = GetVisibilityModifier(node.Modifiers);

            foreach (var identifier in node.Declaration.Variables)
            {
                Emit(string.Format("{0} {1}: {2};", visibility, identifier.GetText(), this.GetMappedType(node.Declaration.Type)), new object[0]);
            }
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            string visibility = GetVisibilityModifier(node.Modifiers);

            Emit(String.Join(" ", visibility, String.Concat(node.Identifier.Text, ":"), this.GetMappedType(node.ReturnType)));

            using (IndentScope())
            {
                base.VisitMethodDeclaration(node);
            }
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            this.Emit("module {0}", new object[] { node.Name.ToString() });
            using (Visitor.EndBlock endBlock = this.IndentScope())
            {
                base.VisitNamespaceDeclaration(node);
            }
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            string mappedType = GetMappedType(node.Type);
            string visibility = Visitor.GetVisibilityModifier(node.Modifiers);

            if (!(node.AccessorList.Accessors.All(ad => ad.Body == null)))
            {
                foreach (var accessor in node.AccessorList.Accessors)
                {
                    var signature = (accessor.Keyword.Kind() != SyntaxKind.GetKeyword ? String.Format("(value: {0})", mappedType) : string.Concat(": ", mappedType));

                    Emit(string.Format("{0} {1} {2}{3}", visibility, accessor.Keyword, node.Identifier.Text, signature));

                    using (IndentScope())
                    {
                        base.Visit(accessor.Body);
                    }
                }
            }
            else
            {
                Emit(string.Join(" ", visibility, string.Concat(node.Identifier.Text, ":"), mappedType));
            }
        }

        internal class EndBlock : IDisposable
        {
            private readonly Visitor _visitor;

            internal EndBlock(Visitor visitor)
            {
                _visitor = visitor;
                _visitor.Emit("{{");
                _visitor._indent = _visitor._indent + 1;
            }

            public void Dispose()
            {
                _visitor._indent = _visitor._indent - 1;
                _visitor.Emit("}}");
            }
        }
    }
}