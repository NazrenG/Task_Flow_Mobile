using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class ChatService:IChatService
    {
        private readonly IChatDal dal;

        public ChatService(IChatDal dal)
        {
            this.dal = dal;
        }

        public async Task AddAsync(Chat chat)
        {
            await dal.Add(chat);
        }
        public async Task DeleteAsync(Chat chat)
        {

            await dal.Delete(chat);
        }
        public async Task<Chat> GetAsync(int id)
        {

            return await dal.GetById(f => f.Id == id);
        }
        public async Task<List<Chat>> GetAllAsync()
        {
            return await dal.GetAll();

        }
        public async Task UpdateAsync(Chat chat)
        {
            await dal.Update(chat);

        }
        public async Task<Chat>GetChatByUserId(string userId)
        {
            return await dal.GetById(c=>c.SenderId==userId||c.ReceiverId==userId);
        }
        public async Task<Chat> GetChatByRecieverId(string userId)
        {
            return await dal.GetById(c =>  c.ReceiverId == userId);
        }
        public async Task<Chat>GetByRecieverAndSenderId(string recieverId,string senderId)
        {
            return await dal.GetById(c=>c.SenderId==senderId|| c.SenderId == recieverId && c.ReceiverId==recieverId||c.ReceiverId==senderId);
        }

    }
}
