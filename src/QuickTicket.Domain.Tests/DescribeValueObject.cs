using System.Collections.Generic;
using Xunit;

namespace QuickTicket.Domain.Tests
{
    public class DescribeValueObject
    {
        public class DescribeGetHashCode
        {
            [Theory]
            [MemberData(nameof(TestData))]
            public void ItShouldGenerateConsistentHashCodes(TestValueObject a,
                TestValueObject b,
                bool expected)
            {
                // Act
                var actual = a.GetHashCode() == b.GetHashCode();
                
                // Assert
                Assert.Equal(expected, actual);
            }

            public static IEnumerable<object[]> TestData()
            {
                {
                    /* Objects with the same properties should be equal */
                    yield return new object[]
                    {
                        new TestValueObject
                        {
                            Property = "A"
                        },
                        new TestValueObject
                        {
                            Property = "A"
                        },
                        true
                    };
                }

                { 
                    /* Objects with different properties should not be equal */
                    yield return new object[]
                    {
                        new TestValueObject
                        {
                            Property = "A"
                        },
                        new TestValueObject
                        {
                            Property = "B"
                        },
                        false
                    };
                }

                { 
                    /* The same instance of the object should be equal */
                    var instance = new TestValueObject
                    {
                        Property = "A"
                    };
                    
                    yield return new object[]
                    {
                        instance,
                        instance,
                        true
                    };
                }
            }
        }
        
        public class DescribeEquals
        {
            [Theory]
            [MemberData(nameof(TestData))]
            public void WhenComparingUsingOperator_ItShouldCompareQuality(TestValueObject a,
                TestValueObject b,
                bool expected)
            {
                // Act
                var actual = a == b;
                
                // Assert
                Assert.Equal(expected, actual);
            }

            [Theory]
            [MemberData(nameof(TestData))]
            public void WhenComparingUsingEqualsMethod_ItShouldCompareEquality(TestValueObject a,
                TestValueObject b,
                bool expected)
            {
                // Act
                var actual = a.Equals(b);
                
                // Assert
                Assert.Equal(expected, actual);
            }

            public static IEnumerable<object[]> TestData()
            {
                {
                    /* Objects with the same properties should be equal */
                    yield return new object[]
                    {
                        new TestValueObject
                        {
                            Property = "A"
                        },
                        new TestValueObject
                        {
                            Property = "A"
                        },
                        true
                    };
                }

                { 
                    /* Objects with different properties should not be equal */
                    yield return new object[]
                    {
                        new TestValueObject
                        {
                            Property = "A"
                        },
                        new TestValueObject
                        {
                            Property = "B"
                        },
                        false
                    };
                }

                { 
                    /* The same instance of the object should be equal */
                    var instance = new TestValueObject
                    {
                        Property = "A"
                    };
                    
                    yield return new object[]
                    {
                        instance,
                        instance,
                        true
                    };
                }
            }
        }

        public class TestValueObject : ValueObject
        {
            public string Property { get; set; }
            
            protected override IEnumerable<object> GetValuesForEqualityCheck()
            {
                yield return Property;
            }
        }
    }
}