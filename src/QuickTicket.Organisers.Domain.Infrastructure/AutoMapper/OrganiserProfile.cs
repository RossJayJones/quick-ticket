using System;
using AutoMapper;
using QuickTicket.Domain;
using QuickTicket.Organisers.Domain.Infrastructure.Documents;

namespace QuickTicket.Organisers.Domain.Infrastructure.AutoMapper
{
    public class OrganiserProfile : Profile
    {
        public OrganiserProfile()
        {
            CreateMap<string, ContactNumber>()
                .ForCtorParam("value", o => o.MapFrom(p => p));
            CreateMap<string, EmailAddress>()
                .ForCtorParam("value", o => o.MapFrom(p => p));
            CreateMap<string, WebsiteUrl>()
                .ForCtorParam("value", o => o.MapFrom(p => p));
            CreateMap<string, CountryCode>()
                .ForCtorParam("value", o => o.MapFrom(p => p));
            CreateMap<string, OrganiserId>()
                .ForCtorParam("value", o => o.MapFrom(p => Guid.Parse(p)));
            CreateMap<AddressDocument, Address>()
                .ReverseMap()
                .ForMember(p => p.CountryCode, o => o.MapFrom(p => p.CountryCode.Value));
            CreateMap<OrganiserDocument, Organiser>()
                .ReverseMap()
                .ForMember(p => p.WebsiteUrl, o => o.MapFrom(p => p.WebsiteUrl.Value))
                .ForMember(p => p.ContactNumber, o => o.MapFrom(p => p.ContactNumber.Value))
                .ForMember(p => p.EmailAddress, o => o.MapFrom(p => p.EmailAddress.Value));
        }
    }
}