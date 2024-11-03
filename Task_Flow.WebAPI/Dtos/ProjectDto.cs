namespace Task_Flow.WebAPI.Dtos
{
    public class ProjectDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? CreatedById { get; set; }//UserId
        public string? Owner { get; set; }//User
        public string? Status { get; set; } 
        public bool IsCompleted { get; set; }
    }
}
