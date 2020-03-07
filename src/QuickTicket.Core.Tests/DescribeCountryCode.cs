using System;
using QuickTicket.Core.Tests.Fixtures;
using Xunit;

namespace QuickTicket.Core.Tests
{
    public class DescribeCountryCode
    {
        [Fact]
        public void ItShouldNotAcceptNullValues()
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => new CountryCode(null));
            
            // Assert
            Assert.Equal(CommonValidationMessages.ArgumentNull("value"), exception.Message);
        }
    }
}