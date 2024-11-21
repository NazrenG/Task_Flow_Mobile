using Microsoft.EntityFrameworkCore;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using TaskFlow.Core.DataAccess.EntityFramework;

namespace Task_Flow.DataAccess.Concrete
{
    public class TeamMemberDal : EFEntityBaseRepository<TaskFlowDbContext, TeamMember>, ITeamMemberDal
    {
        private readonly TaskFlowDbContext _db;
        public TeamMemberDal(TaskFlowDbContext context, TaskFlowDbContext db) : base(context)
        {
            _db = db;   
        }

        public async Task<List<TeamMember>> GetTeamMembers()
        {
          return await _db.TeamMembers.Include(u=>u.User).Include(p=>p.Project).ToListAsync();
        }
    }
}
