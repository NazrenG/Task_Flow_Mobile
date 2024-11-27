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
    public class ProjectActivityDal : EFEntityBaseRepository<TaskFlowDbContext, ProjectActivity>, IProjectActivityDal
    {
        private readonly TaskFlowDbContext _db;
        public ProjectActivityDal(TaskFlowDbContext context,TaskFlowDbContext taskFlowDb) : base(context)
        {
            _db = taskFlowDb;
        }

        public async Task<List<ProjectActivity>> GetAllProjectActivities()
        {
           return await _db.ProjectActivities.Include(p=>p.User).Include(u=>u.Project).ToListAsync();
        }
    }
}
