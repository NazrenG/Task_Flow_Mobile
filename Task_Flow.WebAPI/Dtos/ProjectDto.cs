namespace Task_Flow.WebAPI.Dtos
{
    public class ProjectDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Owner { get; set; }//User
        public bool IsCompleted { get; set; }
    }
}
