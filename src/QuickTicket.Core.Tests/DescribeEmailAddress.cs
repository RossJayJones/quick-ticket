using System;
using QuickTicket.Core.Tests.Fixtures;
using Xunit;

namespace QuickTicket.Core.Tests
{
    public class DescribeEmailAddress
    {
        [Fact]
        public void ItShouldNotAcceptNullValues()
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => new EmailAddress(null));
            
            // Assert
            Assert.Equal(CommonValidationMessages.ArgumentNull("value"), exception.Message);
        }
    }
}