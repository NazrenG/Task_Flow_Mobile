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
        public ProjectActivityDal(TaskFlowDbContext context) : base(context)
        {
        }
    }
}
