using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class TaskAssigne:IEntity
    { 
        public int Id { get; set; } 
        public int TaskForUserId { get; set; }
        public string UserId { get; set; } 
        public DateTime  CreatedAt { get; set; }
        public virtual Work TaskForUser { get; set; }
        public virtual CustomUser User { get; set; }
        public TaskAssigne() {
            CreatedAt = DateTime.Now;
        }
    }
}
