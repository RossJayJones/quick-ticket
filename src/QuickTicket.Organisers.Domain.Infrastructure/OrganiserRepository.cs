using System.Threading.Tasks;
using AutoMapper;
using QuickTicket.Organisers.Domain.Infrastructure.Documents;
using QuickTicket.Storage;

namespace QuickTicket.Organisers.Domain.Infrastructure
{
    public class OrganiserRepository : IOrganiserRepository
    {
        private readonly IDocumentSession _session;
        private readonly IMapper _mapper;
        
        public OrganiserRepository(IDocumentSession session,
            IMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }
        
        public void Add(Organiser organiser)
        {
            var document = _mapper.Map<OrganiserDocument>(organiser);
            _session.Add(document);
        }

        public void Update(Organiser organiser)
        {
            var document = _mapper.Map<OrganiserDocument>(organiser);
            _session.Update(document);
        }

        public async Task<Organiser> LoadAsync(OrganiserId organiserId)
        {
            var documentId = organiserId.Value.ToString();
            var documents = await _session.LoadMany<OrganiserDocument>(new[] { documentId });
            return _mapper.Map<Organiser>(documents[documentId]);
        }
    }
}