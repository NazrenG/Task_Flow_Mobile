using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface IRecentActivityService
    {
        Task<List<RecentActivity>> GetRecentActivities(string userId);
        Task Add(RecentActivity activity);
    }
}
