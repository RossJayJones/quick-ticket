using System.Threading.Tasks;
using AutoMapper;
using QuickTicket.Organisers.Domain;
using QuickTicket.Organisers.Infrastructure.Documents;
using QuickTicket.Storage;

namespace QuickTicket.Organisers.Infrastructure
{
    public class OrganiserRepository : IOrganiserRepository
    {
        private readonly IDocumentSession<OrganiserDocument> _session;
        private readonly IMapper _mapper;
        
        public OrganiserRepository(IDocumentSession<OrganiserDocument> session,
            IMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }
        
        public Task AddAsync(Organiser organiser)
        {
            var document = _mapper.Map<OrganiserDocument>(organiser);
            _session.Add(document);
            return Task.CompletedTask;
        }

        public async Task<Organiser> LoadAsync(OrganiserId organiserId)
        {
            var documentId = organiserId.Value.ToString();
            var documents = await _session.LoadMany(new[] { documentId });
            return _mapper.Map<Organiser>(documents[documentId]);
        }
    }
}