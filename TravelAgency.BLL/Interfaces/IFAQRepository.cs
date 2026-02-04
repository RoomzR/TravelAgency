using TravelAgency.BLL.Entities;

namespace TravelAgency.BLL.Interfaces
{
    public interface IFAQRepository : IRepository<FAQ>
    {
        Task<IEnumerable<FAQ>> GetActiveFAQsAsync();
        Task<IEnumerable<FAQ>> GetFAQsByCategoryAsync(string category);
    }
}