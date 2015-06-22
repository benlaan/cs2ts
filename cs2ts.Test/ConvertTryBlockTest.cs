using System;
using System.Text;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace cs2ts.Tests
{
    [TestFixture]
    public class ConvertTryBlockTest : BaseTest
    {
        [Test]
        public void Can_Convert_Empty_Try_Catch_No_Declared_Exception()
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {
                        try
                        {
                        }
                        catch
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
                "        try",
                "        {",
                "        }",
                "        catch",
                "        {",
                "        }",
                "    }",
                "}",
            };

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_Empty_Try_Catch_With_Declared_Exception()
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {
                        try
                        {
                        }
                        catch (Exception ex)
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
                "        try",
                "        {",
                "        }",
                "        catch (ex)",
                "        {",
                "        }",
                "    }",
                "}",
            };

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_Try_Catch_With_Declared_Exception()
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {
                        try
                        {
                            Console.WriteLine(""Do Work!"");
                            Console.WriteLine(""Do More Work!"");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(""Err!"");
                            Console.WriteLine(""Errors!"");
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
                "        try",
                "        {",
                "            Console.WriteLine(\"Do Work!\");",
                "            Console.WriteLine(\"Do More Work!\");",
                "        }",
                "        catch (ex)",
                "        {",
                "            Console.WriteLine(\"Err!\");",
                "            Console.WriteLine(\"Errors!\");",
                "        }",
                "    }",
                "}",
            };

            Compare(input, expected);
        }
    }
}
