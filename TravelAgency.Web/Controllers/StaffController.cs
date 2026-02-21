using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TravelAgency.DAL.Entities;
using TravelAgency.Web.Models.ViewModels;

[Authorize(Roles = "Director")]
public class StaffController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public StaffController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();
        var staffList = new List<StaffUserViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Client")) 
            {
                staffList.Add(new StaffUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName ?? "Имя не указано", 
                    LastName = user.LastName ?? "",
                    Roles = roles.ToList(),
                    IsLockedOut = await _userManager.IsLockedOutAsync(user)
                });
            }
        }
        return View(staffList);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleBlock(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.Email == "director@travel.com") return RedirectToAction("Index");

        var isLocked = await _userManager.IsLockedOutAsync(user);
        await _userManager.SetLockoutEndDateAsync(user, isLocked ? null : DateTimeOffset.MaxValue);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> ChangeRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.Email == "director@travel.com") return RedirectToAction("Index");

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, role);

        return RedirectToAction("Index");
    }
}