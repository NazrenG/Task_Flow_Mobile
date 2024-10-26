namespace Task_Flow.WebAPI.Dtos
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public string? UserId { get; set; }
         
        public DateTime CreatedDate { get; set; }
    }
}
