using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{

    public class TaskCustomize:IEntity
    {
        public int Id { get; set; } 
        public string? BackColor { get; set; }
        public string? TagColor { get; set; } 
        public int TaskId { get; set; }
        public virtual Work? Task { get; set; }

    }
}
