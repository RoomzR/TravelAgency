using TravelAgency.BLL.DTOs;

namespace TravelAgency.BLL.Interfaces
{
    public interface ITourTypeService
    {
        Task<IEnumerable<TourTypeDTO>> GetAllTourTypesAsync();
        Task<TourTypeDTO?> GetTourTypeByIdAsync(int id);
    }
}