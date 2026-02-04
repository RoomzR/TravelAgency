using TravelAgency.BLL.Entities;

namespace TravelAgency.BLL.Interfaces
{
    public interface IHotelRepository : IRepository<Hotel>
    {
        Task<IEnumerable<Hotel>> GetHotelsByCityAsync(int cityId);
    }
}