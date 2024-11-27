using Task_Flow.Entities.Models;

namespace Task_Flow.WebAPI.Dtos
{
    public class ExtendedProjectListDto
    {
        public int Id { get; set; } 
        public string? Title { get; set; }
        public int TotalTask {  get; set; }
        public int CompletedTask {  get; set; }
        public List<string>? ParticipantsPath { get; set; }
        public DateTime Deadline { get; set; }
        public string? Color { get; set; } 
        public DateTime EndDate{  get; set; }
        public DateTime StartDate { get; set; }
      

    }
}
