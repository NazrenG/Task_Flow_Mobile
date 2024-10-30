using Task_Flow.Core.DataAccess;
using Task_Flow.Entities.Models;


namespace Task_Flow.DataAccess.Abstract
{
  public  interface INotificationDal: IEntityRepository<Notification>
    {
        Task<List<Notification>> GetNotifications();  
    }
}
