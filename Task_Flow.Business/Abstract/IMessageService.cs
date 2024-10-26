using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface IMessageService
    {
        Task<List<Message>> GetMessages();
        Task<Message> GetMessageById(int id);
        Task Add(Message message);
        Task Update(Message message);
        Task Delete(Message message);
    }
}
