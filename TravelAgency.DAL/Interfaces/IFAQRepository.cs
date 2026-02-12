using TravelAgency.DAL.Entities;

namespace TravelAgency.DAL.Interfaces
{
    public interface IFAQRepository : IRepository<FAQ>
    {
        Task<IEnumerable<FAQ>> GetActiveFAQsAsync();
        Task<IEnumerable<FAQ>> GetFAQsByCategoryAsync(string category);
    }
}