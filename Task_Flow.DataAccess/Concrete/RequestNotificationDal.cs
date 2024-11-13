using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using TaskFlow.Core.DataAccess.EntityFramework;

namespace Task_Flow.DataAccess.Concrete
{
    public class RequestNotificationDal : EFEntityBaseRepository<TaskFlowDbContext, RequestNotification>, IRequestNotificationDal
    {
        private readonly TaskFlowDbContext _db;
        public RequestNotificationDal(TaskFlowDbContext context, TaskFlowDbContext db) : base(context)
        {
            _db = db;
        }

        public async Task<List<RequestNotification>> GetRequestNotification()
        {
            return await _db.RequestNotifications.Include(p => p.Sender).Include(r => r.Receiver).ToListAsync();
        }
    }
}
