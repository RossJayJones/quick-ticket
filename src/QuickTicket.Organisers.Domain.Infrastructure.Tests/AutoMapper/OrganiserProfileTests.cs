using AutoMapper;
using QuickTicket.Organisers.Domain.Infrastructure.AutoMapper;
using Xunit;

namespace QuickTicket.Organisers.Domain.Infrastructure.Tests.AutoMapper
{
    public class OrganiserProfileTests
    {
        [Fact]
        public void ItShouldBeValid()
        {
            // Arrange
            var configuration = new MapperConfiguration(cfg =>
            {
                //cfg.ShouldMapProperty = p => p.SetMethod?.IsPrivate == true || p.SetMethod?.IsPublic == true;
                cfg.AddProfile(new OrganiserProfile());
            });
            
            // Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}