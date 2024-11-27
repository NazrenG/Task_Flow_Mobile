using System;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class Message:IEntity
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public DateTime SentDate { get; set; }

        public virtual CustomUser? Sender { get; set; }
        public virtual CustomUser? Receiver { get; set; }
        public Message()
        {
            SentDate = DateTime.Now;
        }
    }
}
