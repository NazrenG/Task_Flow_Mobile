using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface IFriendService
    {
        Task<List<Friend>> GetFriends();
        Task<Friend> GetFriendsById(int id);
        Task Add(Friend friend);
        Task Update(Friend friend);
        Task Delete(Friend friend);
    }
}
