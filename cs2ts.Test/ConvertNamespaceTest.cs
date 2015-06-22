using System;

using NUnit.Framework;

namespace cs2ts
{
    [TestFixture]
    public class ConvertNamespaceTest : BaseTest
    {
        [Test]
        public void Can_Convert_Nested_Namespaces()
        {
            var input = @"

                namespace OuterNamespace.SubNamespace
                {
                    namespace InnerNamespace
                    {
                    }
                }
            @";

            var expected = new[]
            {
                "module OuterNamespace.SubNamespace",
                "{",
                "    module InnerNamespace",
                "    {",
                "    }",
                "}"
            };

            Compare(input, expected);
        }
    }
}
