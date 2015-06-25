using System;
using System.Text;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace cs2ts.Tests
{
    [TestFixture]
    public class ConvertIfBlockTest : BaseTest
    {
        [Test]
        public void Can_Convert_Empty_If_Block()
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {
                        if(true)
                        {
                        }
                    }
                }
            @";

            var expected = new[]
            {
                "public class AClass",
                "{",
                "    public AMethod(): void",
                "    {",
                "        if(true)",
                "        {",
                "        }",
                "    }",
                "}",
            };

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_Empty_If_With_Else()
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {
                        if(true)
                        {
                        }
                        else
                        {
                        }
                    }
                }
            @";

            var expected = new[]
            {
                "public class AClass",
                "{",
                "    public AMethod(): void",
                "    {",
                "        if(true)",
                "        {",
                "        }",
                "        else",
                "        {",
                "        }",
                "    }",
                "}",
            };

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_If_With_ElseIf()
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {
                        if(true)
                        {
                        }
                        else if(false)
                        {
                        }
                        else
                        {
                        }
                    }
                }
            @";

            var expected = new[]
            {
                "public class AClass",
                "{",
                "    public AMethod(): void",
                "    {",
                "        if(true)",
                "        {",
                "        }",
                "        else if (false)",
                "        {",
                "        }",
                "        else",
                "        {",
                "        }",
                "    }",
                "}",
            };

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_If_With_Nested_If()
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {
                        if(true)
                        {
                        }
                        else
                        {
                            if(false)
                            {
                            }
                        }
                    }
                }
            @";

            var expected = new[]
            {
                "public class AClass",
                "{",
                "    public AMethod(): void",
                "    {",
                "        if(true)",
                "        {",
                "        }",
                "        else if (false)",
                "        {",
                "        }",
                "        else",
                "        {",
                "        }",
                "    }",
                "}",
            };

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_If_With_ElseIf_And_Final_Else()
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {
                        if(true)
                        {
                        }
                        else if(false)
                        {
                        }
                    }
                }
            @";

            var expected = new[]
            {
                "public class AClass",
                "{",
                "    public AMethod(): void",
                "    {",
                "        if(true)",
                "        {",
                "        }",
                "        else if (false)",
                "        {",
                "        }",
                "    }",
                "}",
            };

            Compare(input, expected);
        }
    }
}
