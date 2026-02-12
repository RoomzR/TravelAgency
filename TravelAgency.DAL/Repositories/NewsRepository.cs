using Microsoft.EntityFrameworkCore;
using TravelAgency.DAL.Entities;
using TravelAgency.DAL.Interfaces;
using TravelAgency.DAL.Data;

namespace TravelAgency.DAL.Repositories
{
    public class NewsRepository : BaseRepository<NewsArticle>, INewsRepository
    {
        public NewsRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<NewsArticle>> GetLatestNewsAsync(int count)
        {
            return await _context.NewsArticles
                .Include(n => n.Author)
                .OrderByDescending(n => n.CreatedDate)
                .Take(count)
                .ToListAsync();
        }

        public override async Task<NewsArticle?> GetByIdAsync(int id)
        {
            return await _context.NewsArticles
                .Include(n => n.Author)
                .FirstOrDefaultAsync(n => n.Id == id);
        }
    }
}