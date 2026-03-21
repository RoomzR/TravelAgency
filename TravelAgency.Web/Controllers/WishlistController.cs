using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgency.DAL.Data;
using TravelAgency.DAL.Entities;

[Authorize] 
public class WishlistController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public WishlistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [Route("Wishlist/Toggle/{id}")]
    public async Task<IActionResult> Toggle(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var wishlistItem = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.TourId == id);

        bool isInWishlist;

        if (wishlistItem != null)
        {
            _context.Wishlists.Remove(wishlistItem);
            isInWishlist = false;
        }
        else
        {
            _context.Wishlists.Add(new Wishlist
            {
                UserId = userId,
                TourId = id,
                AddedDate = DateTime.Now
            });
            isInWishlist = true;
        }

        await _context.SaveChangesAsync();

        return Json(new { isInWishlist = isInWishlist });
    }
}