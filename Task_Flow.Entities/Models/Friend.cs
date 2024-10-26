using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class Friend:IEntity
    { 
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string? UserId { get; set; }
        public string? UserFriendId { get; set; }

        public virtual CustomUser? User { get; set; }  
        public virtual CustomUser? UserFriend { get; set; }

        public Friend()
        {
            Created = DateTime.Now;
        }

    }
}
