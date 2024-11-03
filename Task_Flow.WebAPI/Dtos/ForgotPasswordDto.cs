namespace Task_Flow.WebAPI.Dtos
{
    public class ForgotPasswordDto
    {
        public string? NameOrEmail { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
