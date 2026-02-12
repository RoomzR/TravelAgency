using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAgency.BLL.DTOs;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Interfaces;

namespace TravelAgency.BLL.Services
{
    public class TourTypeService : ITourTypeService
    {
        private readonly ITourTypeRepository _tourTypeRepository;
        private readonly ILogger<TourTypeService> _logger;
        private readonly IMapper _mapper;

        public TourTypeService(
            ITourTypeRepository tourTypeRepository,
            ILogger<TourTypeService> logger,
            IMapper mapper)
        {
            _tourTypeRepository = tourTypeRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TourTypeDTO>> GetAllTourTypesAsync()
        {
            try
            {
                var tourTypes = await _tourTypeRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<TourTypeDTO>>(tourTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tour types");
                throw;
            }
        }

        public async Task<TourTypeDTO?> GetTourTypeByIdAsync(int id)
        {
            try
            {
                var tourType = await _tourTypeRepository.GetByIdAsync(id);
                return tourType == null ? null : _mapper.Map<TourTypeDTO>(tourType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tour type with ID: {TourTypeId}", id);
                throw;
            }
        }
    }
}