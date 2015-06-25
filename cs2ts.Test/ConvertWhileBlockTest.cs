using System;
using System.Linq;

using NUnit.Framework;

namespace cs2ts.Tests
{
    [TestFixture]
    public class ConvertWhileBlockTest : BaseTest
    {
        [Test]
        public void Can_Convert_Empty_While_Block()
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {
                        while(true)
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
                "        while(true)",
                "        {",
                "        }",
                "    }",
                "}",
            };

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_While_Block_With_Body()
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {
                        while(true)
                        {
                            Console.WriteLine(""!"");
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
                "        while(true)",
                "        {",
                "            Console.WriteLine(\"!\");",
                "        }",
                "    }",
                "}",
            };

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_While_Block_With_Unbraced_Body()
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {
                        while(true)
                            Console.WriteLine(""!"");
                    }
                }
            @";

            var expected = new[]
            {
                "public class AClass",
                "{",
                "    public AMethod(): void",
                "    {",
                "        while(true)",
                "            Console.WriteLine(\"!\");",
                "        }",
                "    }",
                "}",
            };

            Compare(input, expected);
        }
    }
}