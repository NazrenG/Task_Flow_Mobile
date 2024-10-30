using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class Work:IEntity
    {
        public int Id { get; set; } 
        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime ? StartTime { get; set; }
        public DateTime Deadline { get; set; }
        public string? Status { get; set; }//to do,in progress,done

        public string? Priority { get; set; }// Urgent, Primary, Simple
        
        public string? Color { get; set; }
        public int ProjectId { get; set; }

        public string? CreatedById { get; set; }//userId
        //
         
        public virtual CustomUser? CreatedBy { get; set; }
        public virtual Project? Project { get; set; } 
        public virtual List<Comment>? Comments { get; set; } 
        public virtual List<TaskAssigne>? TaskAssignees { get; set; } 
        public virtual List<TaskCustomize>? TaskCustomizes { get; set; }
        
    }
}
