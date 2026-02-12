using TravelAgency.DAL.Entities;

namespace TravelAgency.DAL.Interfaces
{
    public interface INewsRepository : IRepository<NewsArticle>
    {
        Task<IEnumerable<NewsArticle>> GetLatestNewsAsync(int count);
    }
}