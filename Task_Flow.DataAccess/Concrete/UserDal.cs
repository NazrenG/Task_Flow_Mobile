using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using TaskFlow.Core.DataAccess.EntityFramework;

namespace Task_Flow.DataAccess.Concrete
{
    public class UserDal : EFEntityBaseRepository<TaskFlowDbContext, CustomUser>, IUserDal
    {
        public UserDal(TaskFlowDbContext context) : base(context)
        {
        }
    }
}
