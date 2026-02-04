using TravelAgency.BLL.Entities;

namespace TravelAgency.BLL.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(string roleName);
        Task UpdateProfileAsync(ApplicationUser user);
    }
}