namespace Task_Flow.WebAPI.Dtos
{
    public class UserMessageDto
    {
        public string Fullname { get; set; }
        public string Message {  get; set; }
        public int MessageId { get; set; }
        public DateTime SentDate { get; set; }
        public string Status { get; set; } 
        public string Photo {  get; set; }
        public bool? IsOnline { get; set; }
        public bool IsSender { get; set; }

    }
}
