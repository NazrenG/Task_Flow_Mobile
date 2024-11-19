using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
  public  class RequestNotification:IEntity
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public bool IsAccepted {  get; set; }
        public string NotificationType { get; set; }
        public string ProjectName { get; set; }
        public DateTime SentDate { get; set; }

        public virtual CustomUser? Sender { get; set; }
        public virtual CustomUser? Receiver { get; set; }
        public RequestNotification()
        {
            SentDate = DateTime.Now;
        }
    }
}
