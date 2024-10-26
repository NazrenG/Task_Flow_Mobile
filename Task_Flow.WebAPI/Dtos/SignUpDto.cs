namespace Task_Flow.WebAPI.Dtos
{
    public class SignUpDto
    {
     
        public string? Password { get; set; }
        public string? Username { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Occupation { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Image { get; set; }
        public string? Gender { get; set; }
        public bool? IsOnline { get; set; }
        public DateTime? Birthday { get; set; } 
    }
}
