using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cs2ts
{
    public class Transpiler : CSharpSyntaxWalker
    {
        private readonly IList<string> _output;

        private int _indent;

        public Transpiler(string code) : base(SyntaxWalkerDepth.Node)
        {
            _output = new List<string>();
            _indent = 0;

            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            Visit(root);
        }

        private string GetIndentation()
        {
            return new string(' ', _indent * 4);
        }

        private void Emit(string text, params object[] args)
        {
            var indentation = GetIndentation();

            if (!args.Any())
                _output.Add(string.Concat(indentation, text));
            else
                _output.Add(string.Format(string.Concat(indentation, text), args));
        }

        private string GetMappedType(TypeSyntax type)
        {
            if (type.ToString() == "void")
                return "void";

            if (type.ToString().EndsWith("Exception"))
                return type.ToString();

            return type.ToString().StartsWith("int") ? "number" : "string";
        }

        private static string GetVisibilityModifier(SyntaxTokenList tokens)
        {
            return tokens.Any(m => m.Kind() == SyntaxKind.PublicKeyword) ? "public" : "private";
        }

        private Transpiler.BlockScope IndentedBracketScope()
        {
            return new BlockScope(this);
        }

        private Transpiler.BlockScope IndentedBracketScope(SyntaxNode node)
        {
            return new BlockScope(this, node.Kind() == SyntaxKind.Block);
        }

        public void AddIndent()
        {
            _indent += 1;
        }

        public void RemoveIndent()
        {
            _indent -= 1;
        }

        public string ToTypeScript()
        {
            return string.Join(Environment.NewLine, _output);
        }

        public override void VisitBlock(BlockSyntax node)
        {
            foreach (var statement in node.Statements)
            {
                base.Visit(statement);
            }
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            string modifier = GetVisibilityModifier(node.Modifiers);

            Emit(string.Join(" ", modifier, "class", node.Identifier.Text));

            using (IndentedBracketScope())
            {
                base.VisitClassDeclaration(node);
            }
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            string visibility = GetVisibilityModifier(node.Modifiers);

            foreach (var identifier in node.Declaration.Variables)
            {
                Emit(string.Format("{0} {1}: {2};", visibility, identifier.GetText(), GetMappedType(node.Declaration.Type)));
            }
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            string visibility = GetVisibilityModifier(node.Modifiers);

            var parameters = string.Format(
                "({0})",
                node.ParameterList
                    .Parameters
                    .Select(p => string.Format("{0}: {1}", p.Identifier.Text, GetMappedType(p.Type)))
                    .ToCsv()
            );

            var methodSignature = string.Format("{0}{1}:", node.Identifier.Text, parameters);
            Emit(String.Join(" ", visibility, methodSignature, this.GetMappedType(node.ReturnType)));

            using (IndentedBracketScope())
            {
                VisitBlock(node.Body);
            }
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            Emit("module {0}", node.Name.ToString());
            using (IndentedBracketScope())
            {
                base.VisitNamespaceDeclaration(node);
            }
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            string mappedType = GetMappedType(node.Type);
            string visibility = GetVisibilityModifier(node.Modifiers);

            if (!(node.AccessorList.Accessors.All(ad => ad.Body == null)))
            {
                foreach (var accessor in node.AccessorList.Accessors)
                {
                    var signature = (accessor.Keyword.Kind() != SyntaxKind.GetKeyword ? String.Format("(value: {0})", mappedType) : string.Concat(": ", mappedType));

                    Emit(string.Format("{0} {1} {2}{3}", visibility, accessor.Keyword, node.Identifier.Text, signature));

                    using (IndentedBracketScope())
                    {
                        VisitBlock(accessor.Body);
                    }
                }
            }
            else
            {
                Emit(string.Join(" ", visibility, string.Concat(node.Identifier.Text, ":"), mappedType + ";"));
            }
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            Emit("try");
            using (IndentedBracketScope())
            {
                VisitBlock(node.Block);
            }
            foreach (var @catch in node.Catches)
            {
                string arguments = String.Empty;
                if (!(@catch.Declaration == null))
                {
                    if (@catch.Declaration.Identifier != null)
                        arguments = string.Format(" ({0})", @catch.Declaration.Identifier.Text);
                }

                Emit("catch" + arguments);
                using (IndentedBracketScope())
                {
                    VisitBlock(@catch.Block);
                }
            }
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            // TODO: Don't just emit the expression as-is. Need to process the nodes of the expression
            Emit(node.ToString());
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            Emit(node.ToString());
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            var type = node.Type.ToString() != "var" ? GetMappedType(node.Type) : String.Empty;

            if (node.Variables.SeparatorCount == 0)
            {
                foreach (var identifier in node.Variables)
                {
                    var initializer = identifier.Initializer != null ? (" " + identifier.Initializer) : String.Empty;
                    var typeDeclaration = !string.IsNullOrEmpty(type) ? ": " + type : String.Empty;
                    Emit(string.Format("var {0}{1}{2};", identifier.Identifier.Value, typeDeclaration, initializer));
                }
            }
            else
            {
                var prefix = "var ";
                var identifier = node.Variables.Last();
                var initializer = identifier.Initializer != null ? (" " + identifier.Initializer) : String.Empty;
                var typeDeclaration = !string.IsNullOrEmpty(type) ? ": " + type : String.Empty;

                string padding = new string(' ', prefix.Length);
                var separator = String.Concat(",", Environment.NewLine, GetIndentation(), padding);
                var lines = prefix + String.Join(separator, node.Variables.Select(v => v.Identifier.Value).ToList());
                Emit(string.Format("{0}{1}{2};", lines, typeDeclaration, initializer));
            }
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            Emit("if ({0})", node.Condition.ToString());
            using (IndentedBracketScope(node.Statement))
                Visit(node.Statement);

            if (node.Else != null)
            {
                Emit("else");
                using (IndentedBracketScope(node.Else.Statement))
                    Visit(node.Else.Statement);
            }
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            Emit("while ({0})", node.Condition.ToString());
            using (IndentedBracketScope(node.Statement))
                Visit(node.Statement);
        }

        internal class BlockScope : IDisposable
        {
            private readonly Transpiler _visitor;
            private readonly bool _requiresBraces;

            internal BlockScope(Transpiler visitor) : this(visitor, true)
            {
            }

            internal BlockScope(Transpiler visitor, bool requiresBraces)
            {
                _requiresBraces = requiresBraces;
                _visitor = visitor;
                if (_requiresBraces)
                    _visitor.Emit("{");
                _visitor.AddIndent();
            }

            public void Dispose()
            {
                _visitor.RemoveIndent();
                if (_requiresBraces)
                    _visitor.Emit("}");
            }
        }
    }
}