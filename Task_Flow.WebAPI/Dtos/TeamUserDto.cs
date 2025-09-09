using System.Security.Cryptography.X509Certificates;

namespace Task_Flow.WebAPI.Dtos
{
    public class TeamUserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    
        public string Email { get; set; }
   public bool? IsRequested { get; set; }
        public bool? IsSelected { get; set; }

    }
}