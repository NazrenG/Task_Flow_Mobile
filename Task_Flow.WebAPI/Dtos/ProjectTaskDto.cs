namespace Task_Flow.WebAPI.Dtos
{
    public class ProjectTaskDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime Deadline { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Status { get; set; }

        public string? Priority { get; set; }// Urgent, Primary, Simple
        public string? ProjectName { get; set; }
        public string? Color { get; set; }
        public string? CreatedById { get; set; }//userId
    }
}
