
namespace Task_Flow.WebAPI.Dtos
{
    public class WorkDetailsDto
    {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? MemberName { get; set; }
        public string? MemberImage { get; set; }
        public string? MemberMail { get; set; }
        public string? TaskTitle { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime Deadline { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
    }
}