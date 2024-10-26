using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class Quiz:IEntity
    {
        public int Id { get; set; }
        public string? AgeRange { get; set; }
        public string? Profession { get; set; }
        public string? UsagePurpose { get; set; } 
    }
}
