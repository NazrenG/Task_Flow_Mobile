using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface IChatService
    {
        Task AddAsync(Chat chat);
        Task DeleteAsync(Chat chat);
        Task<Chat> GetAsync(int id);
        Task<List<Chat>> GetAllAsync();
        Task UpdateAsync(Chat chat);
        Task<Chat> GetChatByUserId(string userId);
        Task<Chat> GetByRecieverAndSenderId(string recieverId, string senderId);
        Task<List<Chat>> GetAllChatByUserId(string userId);
    }
}
