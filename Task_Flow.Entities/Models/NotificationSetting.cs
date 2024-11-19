using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
  public  class NotificationSetting:IEntity
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public bool FriendshipOffers { get; set; }
        public bool InnovationNewProject { get; set; }
        public bool TaskDueDate { get; set; }
        public bool ProjectCompletationDate { get; set; }
        public bool NewTaskWithInProject { get; set; }
        public virtual CustomUser? User { get; set; }
    }
}
