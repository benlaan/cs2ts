using System;

using NUnit.Framework;

namespace cs2ts
{
    [TestFixture]
    public class ConvertNestedClassTest : BaseTest
    {
        [Test]
        public void Can_Convert_Nested_Class()
        {
            var input = @"

                public class AOuterClass
                {
                    private int _field;
                    public int ANumber { get; set }
                
                    public class AInnerClass
                    {
                        private int _nestedField;
                        public int ANestedNumber { get; set }
                    }
                }
            @";

            var expected = new[]
            {
                "public class AOuterClass",
                "{",
                "    _field: number;",
                "    ANumber: number;",
                "}",
                "module AOuterClass",
                "{",
                "    public class AInnerClass",
                "    {",
                "        _nestedField: number;",
                "        ANestedNumber: number;",
                "    }",
                "}"
            };

            Compare(input, expected);
        }
    }
}
