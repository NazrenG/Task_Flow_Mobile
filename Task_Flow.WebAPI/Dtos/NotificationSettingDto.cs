namespace Task_Flow.WebAPI.Dtos
{
    public class NotificationSettingDto
    {
        public bool FriendshipOffers { get; set; }
        public bool DeadlineReminders { get; set; }
        public bool IncomingComments { get; set; }
        public bool InternalTeamMessages { get; set; }
        public bool NewProjectProposals { get; set; }
    }
}
