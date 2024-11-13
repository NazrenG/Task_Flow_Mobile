using Task_Flow.Core.DataAccess;
using Task_Flow.Entities.Models;


namespace Task_Flow.DataAccess.Abstract
{
    public interface ITaskDal : IEntityRepository<Work>
    {
        public Task<List<int>> GetTaskSummaryByMonthAsync(int projectId,int month,int year);
    }
}
