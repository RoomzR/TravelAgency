using TravelAgency.BLL.Entities;

namespace TravelAgency.BLL.Interfaces
{
    public interface IContactRequestRepository : IRepository<ContactRequest>
    {
        Task<IEnumerable<ContactRequest>> GetPendingRequestsAsync();
        Task<IEnumerable<ContactRequest>> GetRequestsByStatusAsync(string status);
    }
}