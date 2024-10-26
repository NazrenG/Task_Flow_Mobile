using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class Project: IEntity
    {
        public int Id { get; set; } 
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CreatedById { get; set; }//UserId
        public bool IsCompleted { get; set; }   
        public DateTime CreatedAt { get; set; }
        public virtual CustomUser? CreatedBy { get; set; }
        public virtual List<Work>? TaskForUsers { get;set; }
        public virtual List<TeamMember>? TeamMembers { get; set; }

        public Project()
        {
            CreatedAt = DateTime.Now;
        }

    }
}
