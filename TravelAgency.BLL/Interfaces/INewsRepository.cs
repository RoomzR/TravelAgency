using TravelAgency.BLL.Entities;

namespace TravelAgency.BLL.Interfaces
{
    public interface INewsRepository : IRepository<NewsArticle>
    {
        Task<IEnumerable<NewsArticle>> GetLatestNewsAsync(int count);
    }
}