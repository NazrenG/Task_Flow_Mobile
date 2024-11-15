using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
  public  interface INotificationService
    {
        Task<List<Notification>> GetNotifications();
        Task<Notification> GetNotificationById(int id);
        Task Add(Notification notification);
        Task<int> GetCount();
        Task Delete(Notification notification);
    }
}
