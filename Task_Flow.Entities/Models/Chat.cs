using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class Chat:IEntity
    {
        public int Id { get; set; }
        public string? ReceiverId { get; set; }
        public CustomUser? Receiver { get; set; }
        public string? SenderId { get; set; }
        public virtual List<ChatMessage>? Messages { get; set; }
        public Chat()
        {
            Messages = new List<ChatMessage>();
        }
    }
}
