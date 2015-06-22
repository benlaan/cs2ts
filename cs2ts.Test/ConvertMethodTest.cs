using System;
using System.Text;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace cs2ts.Tests
{
    [TestFixture]
    public class ConvertMethodTest : BaseTest
    {
        [Test]
        public void Can_Convert_Method_With_No_Parameters_Returns_Void()
        {
            var input = @"

                public class AClass
                {
                    public void AMethod()
                    {
                    }
                }
            @";

            var expected = new[]
            {
                "public class AClass",
                "{",
                "    public AMethod(): void",
                "    {",
                "    }",
                "}",
            };

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_Method_With_No_Parameters_Returns_Type()
        {
            var input = @"

                public class AClass
                {
                    public string AMethod()
                    {
                    }
                }
            @";

            var expected = new[]
            {
                "public class AClass",
                "{",
                "    public AMethod(): string",
                "    {",
                "    }",
                "}",
            };

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_Method_With_Multiple_Parameters()
        {
            var input = @"

                public class AClass
                {
                    public string AMethod(int index, string name)
                    {
                    }
                }
            @";

            var expected = new[]
            {
                "public class AClass",
                "{",
                "    public AMethod(index: number, name: string): string",
                "    {",
                "    }",
                "}",
            };

            Compare(input, expected);
        }
    }
}
