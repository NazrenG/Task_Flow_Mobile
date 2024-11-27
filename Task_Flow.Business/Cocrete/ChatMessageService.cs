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
    public class ChatMessageService:IChatMessageService
    {
        private readonly IChatMessageDal dal;

        public ChatMessageService(IChatMessageDal dal)
        {
            this.dal = dal;
        }

        public async Task AddAsync(ChatMessage chat)
        {

            await dal.Add(chat);
        }
        public async Task DeleteAsync(ChatMessage chat)
        {

            await dal.Delete(chat);
        }
        public async Task<ChatMessage> GetAsync(int id)
        {
            return await dal.GetById(f => f.Id == id);
        }
        public async Task<List<ChatMessage>> GetAllAsync()
        {
            return await dal.GetAll();
        }
        public async Task UpdateAsync(ChatMessage chat)
        {
            await dal.Update(chat);
        }

        public async Task<List<ChatMessage>> GetAllByChatId(int id)
        {
            var messages = await dal.GetAll(c => c.ChatId == id);
            return messages ?? new List<ChatMessage>();
        }
        public async Task<ChatMessage> GetLatestMessageByChatIdAsync(int chatId)
        {
            return await dal.GetLatestMessageByChatIdAsync(chatId);

        }

    }
}
