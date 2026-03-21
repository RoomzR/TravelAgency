using TravelAgency.DAL.Entities;

namespace TravelAgency.BLL.Interfaces
{
   

    public interface IChatService
    {
        Task<List<ChatMessage>> GetChatHistoryAsync(string userId, string contactId);
    }
}
