using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class Work:IEntity
    {
        public int Id { get; set; } 
        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime ? Created { get; set; }
        public DateTime Deadline { get; set; }
        public string? Status { get; set; }

        public string? Priority { get; set; }// Urgent, Primary, Simple
        public int ProjectId { get; set; }

        public string? CreatedById { get; set; }//userId
        //
         
        public virtual CustomUser? CreatedBy { get; set; }
        public virtual Project? Project { get; set; } 
        public virtual List<Comment>? Comments { get; set; } 
        public virtual List<TaskAssigne>? TaskAssignees { get; set; } 
        public virtual List<TaskCustomize>? TaskCustomizes { get; set; }
        public Work()
        {
            Created=DateTime.Now;
        }
    }
}
