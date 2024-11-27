using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface IChatMessageService
    {
         Task AddAsync(ChatMessage chat);
         Task DeleteAsync(ChatMessage chat);
       Task<ChatMessage> GetAsync(int id);
       Task<List<ChatMessage>> GetAllAsync();
       Task UpdateAsync(ChatMessage chat);
        //Task<ChatMessage> GetByChatId(int id);
        Task<List<ChatMessage>> GetAllByChatId(int id);
        Task<ChatMessage> GetLatestMessageByChatIdAsync(int chatId);
    }
}
