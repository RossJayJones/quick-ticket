using System;
using QuickTicket.Domain.Tests.Fixtures;
using Xunit;

namespace QuickTicket.Domain.Tests
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