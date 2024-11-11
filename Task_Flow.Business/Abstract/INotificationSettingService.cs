using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
  public  interface INotificationSettingService
    {
        Task<NotificationSetting> GetNotificationSetting(string userId);
        Task Update(NotificationSetting notificationSetting);
        Task Add(NotificationSetting notificationSetting);
        Task<NotificationSetting> GetOrCreateNotificationSetting(string userId);
    }
}
