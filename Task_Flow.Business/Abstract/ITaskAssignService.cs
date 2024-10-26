using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface ITaskAssignService
    {
        Task<List<TaskAssigne>> GetTaskAssignes();
        Task<TaskAssigne> GetById(int id);
        Task Add(TaskAssigne assign);
        Task Update(TaskAssigne assign);
        Task Delete(TaskAssigne assign);
    }
}
