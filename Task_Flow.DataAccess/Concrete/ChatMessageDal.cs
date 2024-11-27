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
    public class ChatMessageDal : EFEntityBaseRepository<TaskFlowDbContext, ChatMessage>, IChatMessageDal
    {
        private readonly TaskFlowDbContext _db;
        public ChatMessageDal(TaskFlowDbContext context, TaskFlowDbContext db) : base(context)
        {
            _db = db;
        }
        public async Task<ChatMessage> GetLatestMessageByChatIdAsync(int chatId)
        {
            var list =  _db.ChatMessages
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.SentDate);
            var item=list.FirstOrDefault();
            return item;
                
        }
    }
}
