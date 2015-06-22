using System;
using System.Text;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace cs2ts.Tests
{
    [TestFixture]
    public class ConvertDeclarationStatementTest : BaseTest
    {
        [Test]
        [TestCase("var foo = 1", "var foo = 1")]
        [TestCase("int foo", "var foo: number")]
        [TestCase("int foo = 10", "var foo: number = 10")]
        [TestCase("string foo = 'Hello'", "var foo: string = 'Hello'")]
        [TestCase("string foo = \"Hello\"", "var foo: string = \"Hello\"")]
        public void Can_Convert_Single_Declaration(string declaration, string output)
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {@
                        " + declaration + @";
                    }
                }
            @";

            var expected = new[]
            {
                "public class AClass",
                "{",
                "    public AMethod(): void",
                "    {",
                "        " + output + ";",
                "    }",
                "}",
            };

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_One_Line_Multiple_Auto_Variable_Declaration()
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {
                        var x, y, z = 0;
                    }
                }
            @";

            var expected = new[]
            {
                "public class AClass",
                "{",
                "    public AMethod(): void",
                "    {",
                "        var x,",
                "            y,",
                "            z = 0;",
                "    }",
                "}",
            };

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_Multi_Line_Multiple_Explicit_Variable_Declaration()
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {
                        int index, 
                            zanzibar, 
                            k = 0;
                    }
                }
            @";

            var expected = new[]
            {
                "public class AClass",
                "{",
                "    public AMethod(): void",
                "    {",
                "        var index,",
                "            zanzibar,",
                "            k: number = 0;",
                "    }",
                "}",
            };

            Compare(input, expected);
        }
    }
}
