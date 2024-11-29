using Microsoft.EntityFrameworkCore;
using Task_Flow.Core.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using TaskFlow.Core.DataAccess.EntityFramework;

namespace Task_Flow.DataAccess.Concrete
{
    public class TeamMemberDal : EFEntityBaseRepository<TaskFlowDbContext, TeamMember>, ITeamMemberDal
    {
        private readonly TaskFlowDbContext _context;
        public TeamMemberDal(TaskFlowDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task DeleteAllMembers(List<TeamMember> members)
        {
            foreach (var item in members)
            {
                var deletedEntity = _context.Entry(item);
                deletedEntity.State = EntityState.Deleted;
            }
                await _context.SaveChangesAsync();
        }

        public async Task<List<TeamMember>> GetTeamMembers()
        {
            return await _context.TeamMembers.Include(u => u.User).Include(p => p.Project).ToListAsync();
        }
    }
}
