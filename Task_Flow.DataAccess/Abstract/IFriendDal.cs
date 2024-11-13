using Task_Flow.Core.DataAccess;
using Task_Flow.Entities.Models;


namespace Task_Flow.DataAccess.Abstract
{
    public interface IFriendDal : IEntityRepository<Friend>
    {
        Task<List<Friend>> GetAllFriends(string userId);
    }
}
