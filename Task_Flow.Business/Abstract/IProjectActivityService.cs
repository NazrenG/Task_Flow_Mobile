using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
   public interface IProjectActivityService
    {
        Task Add(ProjectActivity projectActivity );
        //Task Update(Work task);
        Task<List<ProjectActivity>> GetAllByProjectId(int projectId);
        Task Delete(ProjectActivity projectActivity);
    }
}
