using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgency.DAL.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public virtual ApplicationUser Sender { get; set; } 

        public string ReceiverId { get; set; }
        public virtual ApplicationUser Receiver { get; set; }

        public string Message { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
    }
}
