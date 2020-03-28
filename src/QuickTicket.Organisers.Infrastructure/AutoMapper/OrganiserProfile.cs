using AutoMapper;
using QuickTicket.Organisers.Domain;
using QuickTicket.Organisers.Infrastructure.Documents;

namespace QuickTicket.Organisers.Infrastructure.AutoMapper
{
    public class OrganiserProfile : Profile
    {
        public OrganiserProfile()
        {
            CreateMap<Address, AddressDocument>()
                .ForMember(p => p.CountryCode, o => o.MapFrom(p => p.CountryCode.Value))
                .ReverseMap();
            CreateMap<Organiser, OrganiserDocument>()
                .ForMember(p => p.ContactNumber, o => o.MapFrom(p => p.ContactNumber.Value))
                .ForMember(p => p.EmailAddress, o => o.MapFrom(p => p.EmailAddress.Value))
                .ForMember(p => p.WebsiteUrl, o => o.MapFrom(p => p.WebsiteUrl.Value))
                .ForMember(p => p.OrganiserId, o => o.MapFrom(p => p.OrganiserId.Value))
                .ReverseMap();
        }
    }
}