namespace Task_Flow.WebAPI.Dtos
{
    public class NotificationSettingDto
    {
        public bool FriendshipOffers { get; set; }
        public bool InnovationNewProject { get; set; }
        public bool TaskDueDate { get; set; }
        public bool ProjectCompletationDate { get; set; }
        public bool NewTaskWithInProject { get; set; }
    }
}
