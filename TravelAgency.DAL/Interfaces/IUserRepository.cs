using TravelAgency.DAL.Entities;

namespace TravelAgency.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(string roleName);
        Task UpdateProfileAsync(ApplicationUser user);
    }
}