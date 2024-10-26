using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Abstract
{
    public interface ITaskCustomizeService
    {  
        Task<List<TaskCustomize>> GetCustomize ();
        Task<TaskCustomize> GetCustomizeById(int id);
       Task Add(TaskCustomize taskCustomize);
       Task Update(TaskCustomize taskCustomize);
       Task Delete(TaskCustomize taskCustomize);
    }
}
