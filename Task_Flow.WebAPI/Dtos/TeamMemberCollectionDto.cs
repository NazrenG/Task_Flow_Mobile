namespace Task_Flow.WebAPI.Dtos
{
    public class TeamMemberCollectionDto
    {
        public int ProjectId{ get; set; }
        public List<string> Members { get; set; }
    }
}
