using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAgency.BLL.DTOs;
using TravelAgency.DAL.Entities;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Interfaces;

namespace TravelAgency.BLL.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly ILogger<NewsService> _logger;
        private readonly IMapper _mapper;

        public NewsService(
            INewsRepository newsRepository,
            ILogger<NewsService> logger,
            IMapper mapper)
        {
            _newsRepository = newsRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<NewsDTO?> GetNewsByIdAsync(int id)
        {
            try
            {
                var news = await _newsRepository.GetByIdAsync(id);
                return news == null ? null : _mapper.Map<NewsDTO>(news);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news with ID: {NewsId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<NewsDTO>> GetAllNewsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var allNews = await _newsRepository.GetAllAsync();
                var pagedNews = allNews
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                return _mapper.Map<IEnumerable<NewsDTO>>(pagedNews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all news");
                throw;
            }
        }

        public async Task<IEnumerable<NewsDTO>> GetLatestNewsAsync(int count = 3)
        {
            try
            {
                var news = await _newsRepository.GetLatestNewsAsync(count);
                return _mapper.Map<IEnumerable<NewsDTO>>(news);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest news");
                throw;
            }
        }

        public async Task<NewsDTO> CreateNewsAsync(NewsCreateDTO createDto, string authorId)
        {
            try
            {
                var news = _mapper.Map<NewsArticle>(createDto);
                news.AuthorId = authorId;
                news.CreatedDate = DateTime.UtcNow;

                await _newsRepository.CreateAsync(news);

                var createdNews = await _newsRepository.GetByIdAsync(news.Id);
                return _mapper.Map<NewsDTO>(createdNews!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating news");
                throw;
            }
        }

        public async Task<NewsDTO?> UpdateNewsAsync(NewsUpdateDTO updateDto)
        {
            try
            {
                var news = await _newsRepository.GetByIdAsync(updateDto.Id);
                if (news == null) return null;

                _mapper.Map(updateDto, news);
                await _newsRepository.UpdateAsync(news);

                return _mapper.Map<NewsDTO>(news);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating news with ID: {NewsId}", updateDto.Id);
                throw;
            }
        }

        public async Task<bool> DeleteNewsAsync(int id)
        {
            try
            {
                await _newsRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting news with ID: {NewsId}", id);
                throw;
            }
        }
    }
}