namespace Task_Flow.WebAPI.Dtos
{
    public class FriendForMessageDto
    {
        public string? RecentMessage { get; set; }
        public bool isReciever { get; set; }
        public string? FriendEmail { get; set; }
        public string? FriendImg { get; set; }
        public string? FriendFullname { get; set; }
    }
}
