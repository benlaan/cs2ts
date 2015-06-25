using System;

using NUnit.Framework;

namespace cs2ts.Tests
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
            ";

            var expected = @"

                module OuterNamespace.SubNamespace
                {
                    module InnerNamespace
                    {
                    }
                }
            ";

            Compare(input, expected);
        }
    }
}
