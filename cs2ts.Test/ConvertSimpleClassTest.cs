using System;
using System.Text;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace cs2ts.Tests
{
    [TestFixture]
    public class ConvertSimpleClassTest : BaseTest
    {
        [Test]
        public void Can_Convert_Simple_Class_With_Single_Field()
        {
            var input = @"

                namespace ANamespace
                {
                    public class AClass
                    {
                        private int _field;
                    }
                }
            ";

            var expected = @"

                module ANamespace
                {
                    public class AClass
                    {
                        private _field: number;
                    }
                }
            ";

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_Simple_Class_With_Multiple_Fields()
        {
            var input = @"

                namespace ANamespace
                {
                    public class AClass
                    {
                        private int _field1;
                        private int _field2;
                        private int _field3;
                    }
                }
             ";

            var expected = @"

                module ANamespace
                {
                    public class AClass
                    {
                        private _field1: number;
                        private _field2: number;
                        private _field3: number;
                    }
                }
            ";

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_Simple_Class_With_Multiple_Fields_Within_One_Declaration()
        {
            var input = @"

                namespace ANamespace
                {
                    public class AClass
                    {
                        private int _field1, _field2, _field3;
                    }
                }
             ";

            var expected = @"

                module ANamespace
                {
                    public class AClass
                    {
                        private _field1: number;
                        private _field2: number;
                        private _field3: number;
                    }
                }
             ";

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_Simple_Class_With_No_Properties()
        {
            var input = @"

                namespace ANamespace
                {
                    public class AClass
                    {
                    }
                }
             ";

            var expected = @"

                module ANamespace
                {
                    public class AClass
                    {
                    }
                }
             ";

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_Simple_Class_With_One_Auto_Property()
        {
            var input = @"

                namespace ANamespace
                {
                    public class AClass
                    {
                        public int AProperty { get; set; }
                    }
                }
             ";

            var expected = @"

                module ANamespace
                {
                    public class AClass
                    {
                        public AProperty: number;
                    }
                }
             ";

            Compare(input, expected);
        }

        [Test]
        public void Can_Convert_Simple_Class_With_One_field_Backed_Property()
        {
            var input = @"

                namespace ANamespace
                {
                    public class AClass
                    {
                        private int _field;
                        public int AProperty
                        {
                            get { return _field; }
                            set { _field = value; }
                        }
                    }
                }
             ";

            var expected = @"

                module ANamespace
                {
                    public class AClass
                    {
                        private _field: number;
                        public get AProperty: number
                        {
                            return _field;
                        }
                        public set AProperty(value: number)
                        {
                            _field = value;
                        }
                    }
                }
            ";

            Compare(input, expected);
        }
    }
}
