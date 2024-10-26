namespace Task_Flow.WebAPI.Dtos
{
    public class ProjectDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CreatedById { get; set; }//UserId
        public bool IsCompleted { get; set; }
    }
}
