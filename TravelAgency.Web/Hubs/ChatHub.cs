using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel;
using TravelAgency.DAL.Data;
using TravelAgency.DAL.Entities;
namespace TravelAgency.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatHub(ApplicationDbContext context , UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = _userManager.GetUserId(Context.User);
            var sender = await _userManager.FindByIdAsync(senderId);

            var chatMsg = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                SentAt = DateTime.Now
            };

            _context.ChatMessages.Add(chatMsg);
            await _context.SaveChangesAsync();

            await Clients.User(receiverId).SendAsync("NotifyNewMessage", new
            {
                senderName = sender.FirstName,
                text = message.Substring(0, Math.Min(message.Length, 20)) + "..."
            });

            var messageData = new
            {
                senderId = senderId,
                senderName = $"{sender.FirstName} {sender.LastName}",
                text = message,
                time = chatMsg.SentAt.ToString("HH:mm")
            };

            await Clients.User(receiverId).SendAsync("ReceiveMessage", messageData);

          
            await Clients.Caller.SendAsync("ReceiveMessage", messageData);
        }

        public async Task Typing(string receiverId)
        {
            var senderId = _userManager.GetUserId(Context.User);
            await Clients.User(receiverId).SendAsync("UserTyping", senderId);
        }

    }
}
