using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationDal notificationDal;

        public NotificationService(INotificationDal notificationDal)
        {
            this.notificationDal = notificationDal;
        }

        public async Task Add(Notification notification)
        {
          await notificationDal.Add(notification);  
        }

        public async Task Delete(Notification notification)
        {
           await notificationDal.Delete(notification);  
        }

        public async Task<Notification> GetNotificationById(int id)
        {
            return await notificationDal.GetById(p=>p.Id==id);
        }

        public async Task<List<Notification>> GetNotifications()
        {
            return await notificationDal.GetNotifications();
        }

        public async Task<int> GetCount()
        {
           var list= await notificationDal.GetAll();
            return list.Count();
        }
    }
}
