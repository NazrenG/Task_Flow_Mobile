using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Abstract
{
    public interface ITaskService
    {
        Task<List<Work>> GetTasks();
        Task<Work> GetTaskById(int id);
      Task Add(Work task);
      Task Update(Work task);
      Task Delete(Work task); 

    }
}
