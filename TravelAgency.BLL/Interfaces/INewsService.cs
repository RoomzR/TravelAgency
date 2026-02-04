using TravelAgency.BLL.DTOs;

namespace TravelAgency.BLL.Interfaces
{
    public interface INewsService
    {
        Task<NewsDTO?> GetNewsByIdAsync(int id);
        Task<IEnumerable<NewsDTO>> GetAllNewsAsync(int page = 1, int pageSize = 10);
        Task<IEnumerable<NewsDTO>> GetLatestNewsAsync(int count = 3);
        Task<NewsDTO> CreateNewsAsync(NewsCreateDTO createDto, string authorId);
        Task<NewsDTO?> UpdateNewsAsync(NewsUpdateDTO updateDto);
        Task<bool> DeleteNewsAsync(int id);
    }
}