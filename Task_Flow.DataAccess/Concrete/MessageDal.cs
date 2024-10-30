using Microsoft.EntityFrameworkCore;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using TaskFlow.Core.DataAccess.EntityFramework;

namespace Task_Flow.DataAccess.Concrete
{
    public class MessageDal : EFEntityBaseRepository<TaskFlowDbContext, Message>, IMessageDal
    {
        private readonly TaskFlowDbContext _db; 
        public MessageDal(TaskFlowDbContext context, TaskFlowDbContext db) : base(context)
        {
            _db = db;
        }

        public async Task<List<Message>> GetMessages()
        {
            return await _db.Messages.Include(p=>p.Sender).Include(r=>r.Receiver).ToListAsync();   
        }
    }
}
