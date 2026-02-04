using Microsoft.EntityFrameworkCore;
using TravelAgency.BLL.Entities;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Data;

namespace TravelAgency.DAL.Repositories
{
    public class PromoCodeRepository : BaseRepository<Promocode>, IPromoCodeRepository
    {
        public PromoCodeRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Promocode?> GetByCodeAsync(string code)
        {
            return await _context.PromoCodes
                .FirstOrDefaultAsync(p => p.Code.ToUpper() == code.ToUpper() && p.IsActive);
        }

        public async Task<bool> IsCodeValidAsync(string code)
        {
            var promoCode = await GetByCodeAsync(code);
            if (promoCode == null) return false;

            var now = DateTime.UtcNow;
            return promoCode.IsActive &&
                   promoCode.ValidFrom <= now &&
                   promoCode.ValidTo >= now &&
                   (!promoCode.MaxUses.HasValue || promoCode.CurrentUses < promoCode.MaxUses.Value);
        }

        public async Task IncrementUsesAsync(int id)
        {
            var promoCode = await _context.PromoCodes.FindAsync(id);
            if (promoCode != null)
            {
                promoCode.CurrentUses++;
                await _context.SaveChangesAsync();
            }
        }
    }
}