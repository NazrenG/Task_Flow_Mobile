using Microsoft.EntityFrameworkCore;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using TaskFlow.Core.DataAccess.EntityFramework;

namespace Task_Flow.DataAccess.Concrete
{
    public class FriendDal : EFEntityBaseRepository<TaskFlowDbContext, Friend>,IFriendDal
    {
        private readonly TaskFlowDbContext _db;
        public FriendDal(TaskFlowDbContext context, TaskFlowDbContext db) : base(context)
        {
            _db = db;
        }

        public async Task<List<Friend>> GetAllFriends(string userId)
        {
          return await _db.Friends.Include(u=>u.UserFriend).Where(i=>i.UserId==userId).ToListAsync();    
        }
    }
}
