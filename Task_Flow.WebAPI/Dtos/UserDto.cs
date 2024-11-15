namespace Task_Flow.WebAPI.Dtos
{
    public class UserDto
    {  
        public string? Fullname { get; set; } 
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Occupation { get; set; }
        public string? Country { get; set; } 
    //    public IFormFile? Image { get; set; }
        public string? Gender { get; set; } 
        public DateTime? Birthday { get; set; }
        public string? Image {  get; set; }
    }
}
