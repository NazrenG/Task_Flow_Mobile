using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class TaskAssigneService:ITaskAssignService
    {
        private readonly ITaskAssignDal taskAssignDal;

        public TaskAssigneService(ITaskAssignDal taskAssignDal)
        {
            this.taskAssignDal = taskAssignDal;
        }

        public async Task Add(TaskAssigne assign)
        {
           await taskAssignDal.Add(assign);
        }

        public async Task Delete(TaskAssigne assign)
        {
            await taskAssignDal.Delete(assign);
        }

        public async Task<TaskAssigne> GetById(int id)
        {
            return await taskAssignDal.GetById(t=>t.Id==id);
        }

        public async Task<List<TaskAssigne>> GetTaskAssignes()
        {
            return await taskAssignDal.GetAll();
        }

        public async Task Update(TaskAssigne assign)
        {
            await taskAssignDal.Update(assign); 
        }
    }
}
