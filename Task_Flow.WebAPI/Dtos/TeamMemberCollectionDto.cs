namespace Task_Flow.WebAPI.Dtos
{
    public class TeamMemberCollectionDto
    {
        public int ProjectId{ get; set; }
        public List<TeamUserDto> Members { get; set; }
    }
}
