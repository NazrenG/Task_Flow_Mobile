namespace Task_Flow.WebAPI.Dtos
{
    public class FriendDto
    {
        public string Id { get; set; }
        public string? FriendName { get; set; }
        public string? FriendOccupation { get; set; }
        public string? FriendPhone { get; set; }
        public string? FriendEmail { get; set; }
        public bool IsFriend { get; set; }
        public bool HasRequestPending { get; set; }
        public string? FriendPhoto { get; set; }
        public bool? IsOnline { get; set; }
        public bool? CheckFriend { get; set; }

    }
}
