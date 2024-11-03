using Microsoft.EntityFrameworkCore;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using TaskFlow.Core.DataAccess.EntityFramework;

namespace Task_Flow.DataAccess.Concrete
{
    public class NotificationDal : EFEntityBaseRepository<TaskFlowDbContext, Notification>, INotificationDal
    {
        private readonly TaskFlowDbContext _db;
        public NotificationDal(TaskFlowDbContext context) : base(context)
        {
            _db = context;
        }

        public async Task<List<Notification>> GetNotifications()
        {
         return await  _db.Notifications.Include(r=>r.User).ToListAsync();   
        }
    }
}
