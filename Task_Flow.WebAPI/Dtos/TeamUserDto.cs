namespace Task_Flow.WebAPI.Dtos
{
    public class TeamUserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Occupation { get; set; }
        public string Status { get; set; }
        public bool? IsOnline { get; set; }

    }
}