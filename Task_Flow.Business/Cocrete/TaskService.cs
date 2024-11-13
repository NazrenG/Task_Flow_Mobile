using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Concrete
{
    public class TaskService : ITaskService
    {
        private readonly ITaskDal dal;

        public TaskService(ITaskDal dal)
        {
            this.dal = dal;
        }

        public async Task Add(Work task)
        {
            await dal.Add(task);
        }

        public async Task Delete(Work task)
        {
            await dal.Delete(task);
        }

        public async Task<List<Work>> GetDoneTask(string userId)
        {
                return await dal.GetAll(t => t.Status!.ToLower() == "done" && t.CreatedById==userId);
        }

        public async Task<List<Work>> GetInProgressTask(string userId)
        {
            return await dal.GetAll(t => t.Status!.ToLower() == "in progress" && t.CreatedById == userId);
        }

        public async Task<Work> GetTaskById(int id)
        {
            return await dal.GetById(f => f.Id == id);
        }

        public async Task<List<Work>> GetTasks(string userId)
        {
            return await dal.GetAll(u=>u.CreatedById==userId);
        }

        public async Task<List<Work>> GetToDoTask(string userId)
        {
            return await dal.GetAll(t => t.Status!.ToLower() == "to do" && t.CreatedById == userId);
        }

        public async Task Update(Work task)
        {
            await dal.Update(task);
        }
    }
}
