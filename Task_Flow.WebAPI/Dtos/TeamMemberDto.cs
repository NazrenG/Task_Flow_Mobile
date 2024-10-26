namespace Task_Flow.WebAPI.Dtos
{
    public class TeamMemberDto
    {
        public int ProjectId { get; set; }

        public string? UserId { get; set; }
        public string? Role { get; set; }
    }
}
