using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class RecentActivityService : IRecentActivityService
    {
        private readonly IRecentActivityDal activityDal;

        public RecentActivityService(IRecentActivityDal activityDal)
        {
            this.activityDal = activityDal;
        }

        public async Task Add(RecentActivity activity)
        {
            await activityDal.Add(activity);
        }

        public async Task<List<RecentActivity>> GetRecentActivities(string userId)
        {
            return await activityDal.GetAll(u => u.UserId == userId);
        }
    }
}
