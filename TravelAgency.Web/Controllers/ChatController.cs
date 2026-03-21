using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgency.DAL.Data;
using TravelAgency.DAL.Entities;
using TravelAgency.Web.Models.ViewModels;
using TravelAgency.BLL.Services;
using TravelAgency.BLL.Interfaces;


[Authorize]
public class ChatController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IChatService _chatService;

    public ChatController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IChatService chatService)
    {
        _context = context;
        _userManager = userManager;
        _chatService = chatService;
    }

    public async Task<IActionResult> MyDialogs()
    {
        var currentUserId = _userManager.GetUserId(User);


        var dialogs = await _context.ChatMessages
            .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
            .GroupBy(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
            .Select(g => new DialogViewModel
            {
                UserId = g.Key,
                FullName = _context.Users
                    .Where(u => u.Id == g.Key)
                    .Select(u => u.FirstName + " " + u.LastName)
                    .FirstOrDefault() ?? "Пользователь",
                LastMessage = g.OrderByDescending(m => m.SentAt).Select(m => m.Message).FirstOrDefault(),
                LastMessageTime = g.Max(m => m.SentAt)
            })
            .OrderByDescending(d => d.LastMessageTime)
            .ToListAsync();

        return View(dialogs);
    }

    [Authorize]
    public async Task<IActionResult> Index(string receiverId)
    {
        if (string.IsNullOrEmpty(receiverId))
        {
            return RedirectToAction("MyDialogs");
        }

        var currentUserId = _userManager.GetUserId(User);

        var messages = await _chatService.GetChatHistoryAsync(currentUserId, receiverId);

        var receiver = await _userManager.FindByIdAsync(receiverId);


        if (receiver == null) return NotFound();

        ViewBag.ReceiverName = $"{receiver.FirstName} {receiver.LastName}";
        ViewBag.ReceiverId = receiverId;
        ViewBag.CurrentUserId = currentUserId;

        return View(messages);
    }
    [Authorize]
    public async Task<IActionResult> Support()
    {
        var admin = (await _userManager.GetUsersInRoleAsync("Admin")).FirstOrDefault();

        if (admin == null)
        {   
            TempData["ErrorMessage"] = "Служба поддержки временно недоступна.";
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Index", new { receiverId = admin.Id });
    }

}