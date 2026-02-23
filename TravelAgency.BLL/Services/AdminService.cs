using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelAgency.BLL.DTOs;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Data;
using TravelAgency.DAL.Entities;
using TravelAgency.DAL.Enums;

namespace TravelAgency.BLL.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context; 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public AdminService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<AdminDashboardDTO> GetDashboardDataAsync()
        {
            var bookings = await _context.Set<Booking>()
                .Include(b => b.Tour)
                .Include(b => b.Client)
                .ToListAsync();

            return new AdminDashboardDTO
            {
                TotalUsersCount = await _context.Set<ApplicationUser>().CountAsync(),
                TotalToursCount = await _context.Set<Tour>().CountAsync(),
                ActiveBookingsCount = bookings.Count(b => b.Status != BookingStatus.Cancelled),
                TotalRevenue = bookings.Where(b => b.Status == BookingStatus.Confirmed).Sum(b => b.FinalPrice),

                RecentBookings = _mapper.Map<List<BookingDTO>>(bookings.OrderByDescending(b => b.BookingDate).Take(5)),

                NewUsers = _mapper.Map<List<UserDTO>>(await _context.Set<ApplicationUser>()
                    .OrderByDescending(u => u.RegistrationDate)
                    .Take(5).ToListAsync())
            };
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDTO>>(users);

            foreach (var dto in userDtos)
            {
                var user = users.First(u => u.Id == dto.Id);

                dto.IsBlocked = await _userManager.IsLockedOutAsync(user);

                var roles = await _userManager.GetRolesAsync(user);
                dto.RoleName = roles.FirstOrDefault() ?? "Client";
            }

            return userDtos;
        }

        public async Task<bool> UpdateUserRoleAsync(string userId, string newRoleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await _roleManager.RoleExistsAsync(newRoleName)) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var result = await _userManager.AddToRoleAsync(user, newRoleName);
            return result.Succeeded;
        }

        public async Task<IEnumerable<BookingDTO>> GetAllBookingsAsync()
        {
            var bookings = await _context.Set<Booking>()
                .Include(b => b.Tour)
                .Include(b => b.Client)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }

        public async Task ApproveReviewAsync(int reviewId)
        {
            var review = await _context.Set<Review>().FindAsync(reviewId);
            if (review != null)
            {
                review.IsApproved = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateFAQAsync(FAQCreateDTO faqDto)
        {
            var faq = _mapper.Map<FAQ>(faqDto);
            await _context.Set<FAQ>().AddAsync(faq);
            await _context.SaveChangesAsync();
        }

        public async Task CreatePromoCodeAsync(PromoCodeCreateDTO promoDto)
        {
            var promo = _mapper.Map<Promocode>(promoDto);
            await _context.Set<Promocode>().AddAsync(promo);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ContactRequestDTO>> GetContactRequestsAsync()
        {
            var requests = await _context.Set<ContactRequest>().OrderByDescending(r => r.CreatedDate).ToListAsync();
            return _mapper.Map<IEnumerable<ContactRequestDTO>>(requests);
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, string status)
        {
            var booking = await _context.Set<Booking>().FindAsync(bookingId);
            if (booking == null) return false;

            if (Enum.TryParse<BookingStatus>(status, out var newStatus))
            {
                booking.Status = newStatus;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> BlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));

            return result.Succeeded;
        }

        public async Task<bool> UnblockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            return result.Succeeded;
        }
        public async Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync()
        {
            var reviews = await _context.Set<Review>()
                .Include(r => r.Client)
                .Include(r => r.Tour)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        }

        public async Task ToggleReviewApprovalAsync(int reviewId)
        {
            var review = await _context.Set<Review>().FindAsync(reviewId);
            if (review != null)
            {
                review.IsApproved = !review.IsApproved;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<PromoCodeDTO>> GetAllPromoCodesAsync()
        {
            var promos = await _context.Set<Promocode>()
                .OrderByDescending(p => p.ValidTo)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PromoCodeDTO>>(promos);
        }

        public async Task TogglePromoCodeStatusAsync(int promoId)
        {
            var promo = await _context.Set<Promocode>().FindAsync(promoId);
            if (promo != null)
            {
                promo.IsActive = !promo.IsActive; 
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeletePromoCodeAsync(int id)
        {
            var p = await _context.Set<Promocode>().FindAsync(id);
            if (p != null)
            {
                _context.Set<Promocode>().Remove(p); await _context.SaveChangesAsync();
            }
        }
       
    }
}