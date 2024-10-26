using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using TaskFlow.Core.DataAccess.EntityFramework;

namespace Task_Flow.DataAccess.Concrete
{
    public class MessageDal : EFEntityBaseRepository<TaskFlowDbContext, Message>, IMessageDal
    {
        public MessageDal(TaskFlowDbContext context) : base(context)
        {
        }
    }
}
