using System.ComponentModel.DataAnnotations;

namespace Task_Flow.WebAPI.Dtos
{
    public class SignUpDto
    {

        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Firstname { get; set; }
        [Required]
        public string? Lastname { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Occupation { get; set; }
        [Required]
        public string? Country { get; set; }
        [Required]
        public string? City { get; set; }
        public string? Image { get; set; }
        [Required]
        public string? Gender { get; set; }
        public bool? IsOnline { get; set; }
        [Required]
        public DateTime? Birthday { get; set; }
    }
}
