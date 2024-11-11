using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class NotificationSettingService : INotificationSettingService
    {
        private readonly INotificationSettingsDal dal;

        public NotificationSettingService(INotificationSettingsDal dal)
        {
            this.dal = dal;
        }

        public async Task Add(NotificationSetting notificationSetting)
        {
           await dal.Add(notificationSetting);
        }

        public async Task<NotificationSetting> GetNotificationSetting(string userId)
        {
         return  await dal.GetById(u=>u.UserId == userId);    

        }

        public async Task Update(NotificationSetting notificationSetting)
        {
           await dal.Update(notificationSetting);   
        }

        public async Task<NotificationSetting> GetOrCreateNotificationSetting(string userId)
        {
            var notificationSetting = await dal.GetById(u => u.UserId == userId);

            if (notificationSetting == null)
            {
                notificationSetting = new NotificationSetting
                {
                    UserId = userId,
                    FriendshipOffers = false,
                    DeadlineReminders = false,
                    IncomingComments = false,
                    InternalTeamMessages = false,
                    NewProjectProposals = false
                };

                await dal.Add(notificationSetting); 
            }

            return notificationSetting;
        }

    }
}
