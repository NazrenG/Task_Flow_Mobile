namespace Task_Flow.WebAPI.Dtos
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public DateTime SentDate { get; set; }
    }
}
