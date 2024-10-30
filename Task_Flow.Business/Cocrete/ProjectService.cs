using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Concrete
{
    public class ProjectService:IProjectService
    {
        private readonly IProjectDal dal;

        public ProjectService(IProjectDal dal)
        {
            this.dal = dal;
        }

        public async Task Add(Project project)
        {
          await dal.Add(project);
        }

        public async Task Delete(Project project)
        {
          await dal.Delete(project);
        }

        public async Task<Project> GetProjectById(int id)
        {
            return await dal.GetById(p => p.Id == id);
        }

        public async Task<List<Project>> GetProjects(string userId)
        {
           return await dal.GetAll(u=>u.CreatedById==userId);
        }

        public async Task Update(Project project)
        {
           await dal.Update(project);
        }

        public async Task<int> GetUserProjectCount(string userId)
        {
            var list = await dal.GetAll(p => p.CreatedById == userId);
            return list.Count();    
        }

        public async Task<List<Project>> GetOnGoingProject(string userId)
        {
           return await dal.GetAll(u=>u.CreatedById == userId && u.Status!.ToLower()=="on going");
        }

        public async Task<List<Project>> GetPendingProject(string userId)
        {
            return await dal.GetAll(u => u.CreatedById == userId && u.Status!.ToLower() == "pending");

        }

        public async Task<List<Project>> GetCompletedTask(string userId)
        {
            return await dal.GetAll(u => u.CreatedById == userId && u.Status!.ToLower() == "completed");

        }
    }
}
