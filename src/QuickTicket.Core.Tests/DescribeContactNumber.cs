using System;
using QuickTicket.Core.Tests.Fixtures;
using Xunit;

namespace QuickTicket.Core.Tests
{
    public class DescribeContactNumber
    {
        [Fact]
        public void ItShouldNotAcceptNullValues()
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => new ContactNumber(null));
            
            // Assert
            Assert.Equal(CommonValidationMessages.ArgumentNull("value"), exception.Message);
        }
    }
}