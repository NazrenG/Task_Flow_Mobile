using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using TaskFlow.Core.DataAccess.EntityFramework;

namespace Task_Flow.DataAccess.Concrete
{
    public class UserTaskDal : EFEntityBaseRepository<TaskFlowDbContext, UserTask>, IUserTaskDal
    {
        public UserTaskDal(TaskFlowDbContext context) : base(context)
        {
        }
    }
}
