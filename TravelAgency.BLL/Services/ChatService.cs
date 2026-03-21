using Microsoft.EntityFrameworkCore;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Data;
using TravelAgency.DAL.Entities;

namespace TravelAgency.BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatMessage>> GetChatHistoryAsync(string userId, string contactId)
        {
            return await _context.ChatMessages
                .Where(m => (m.SenderId == userId && m.ReceiverId == contactId) ||
                            (m.SenderId == contactId && m.ReceiverId == userId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}
