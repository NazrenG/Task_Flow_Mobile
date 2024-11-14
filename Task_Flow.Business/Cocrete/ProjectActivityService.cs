using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public  class ProjectActivityService : IProjectActivityService
    {
        private readonly IProjectActivityDal dal;

        public ProjectActivityService(IProjectActivityDal dal)
        {
            this.dal = dal;
        }

        public async Task Add(ProjectActivity projectActivity)
        {
            await dal.Add(projectActivity);
        }

        public async Task Delete(ProjectActivity projectActivity)
        {
            await dal.Delete(projectActivity);
        }

        public async Task<List<ProjectActivity>> GetAllByProjectId(int projectId)
        {
            return await dal.GetAll(p => p.ProjectId==projectId);
        }

    }
}
