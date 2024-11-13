using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.DataAccess.Concrete;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class RequestNotificationService:IRequestNotificationService
    {
        private readonly IRequestNotificationDal requestNotificationDal;

    public RequestNotificationService(IRequestNotificationDal requestNotificationDal)
    {
        this.requestNotificationDal = requestNotificationDal;
    }

    public async Task Add(RequestNotification requestNotification)
    {
        await requestNotificationDal.Add(requestNotification);
    }

    public async Task Delete(RequestNotification RequestNotification)
    {
        await requestNotificationDal.Delete(RequestNotification);
    }

    public async Task<RequestNotification> GetRequestNotificationById(int id)
    {
        return await requestNotificationDal.GetById(f => f.Id == id);
    }

    public async Task<List<RequestNotification>> GetRequestNotifications(string receiverId)
    {
       var list=await requestNotificationDal.GetRequestNotification();
            return list.Where(u=>u.ReceiverId == receiverId).ToList();  
    }

    public async Task Update(RequestNotification RequestNotification)
    {
        await requestNotificationDal.Update(RequestNotification);
    }
}


}
