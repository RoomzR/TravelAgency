using TravelAgency.BLL.Entities;

namespace TravelAgency.BLL.Interfaces
{
    public interface IPromoCodeRepository : IRepository<Promocode>
    {
        Task<Promocode?> GetByCodeAsync(string code);
        Task<bool> IsCodeValidAsync(string code);
        Task IncrementUsesAsync(int id);
    }
}