using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface IUserTaskService
    {
        Task<List<UserTask>> GetUserTasks(string userId);
        Task<UserTask> GetById(int id);
        Task Add(UserTask assign);
        Task Update(UserTask assign);
        Task Delete(UserTask assign);
        Task<List<UserTask>> GetToDoTask(string userId);
        Task<List<UserTask>> GetInProgressTask(string userId);
        Task<List<UserTask>> GetDoneTask(string userId);
        Task<bool> CheckTaskName(string title);
    }
}
