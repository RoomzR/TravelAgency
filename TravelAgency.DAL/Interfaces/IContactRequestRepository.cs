using TravelAgency.DAL.Entities;

namespace TravelAgency.DAL.Interfaces
{
    public interface IContactRequestRepository : IRepository<ContactRequest>
    {
        Task<IEnumerable<ContactRequest>> GetPendingRequestsAsync();
        Task<IEnumerable<ContactRequest>> GetRequestsByStatusAsync(string status);
    }
}