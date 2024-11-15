using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class Notification:IEntity
    { 
        public int Id { get; set; }  
        public string? Text { get; set; } 
        public string? UserId { get; set; } 
        public bool IsCalendarMessage { get; set; } 
        public DateTime? Created { get; set; } 
        public virtual CustomUser? User { get; set; }
        public Notification()
        {
            Created = DateTime.Now;
            IsCalendarMessage = false;
        }
 
    }
}
