using Microsoft.EntityFrameworkCore;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly ITeamMemberDal dal;

        public TeamMemberService(ITeamMemberDal dal)
        {
            this.dal = dal;
        }

        public async Task<List<TeamMember>> TeamMembers()
        {
            return await dal.GetAll();
        }

        public async Task<TeamMember> GetTaskMemberById(int id)
        {
            return await dal.GetById(f => f.Id == id);
        }

        public async Task<List<TeamMember>> GetTaskMemberListById(int id)
        {
            var list = await dal.GetTeamMembers();
            return list.Where(p=>p.ProjectId==id).ToList();
        }


        public async Task<List<TeamMember>> GetProjectListByUserIdAsync(string id)
        {
            var list = await dal.GetAll(t => t.UserId == id);
            return list;
        }

        public async System.Threading.Tasks.Task Add(TeamMember teamMember)
        {
            await dal.Add(teamMember);
        }

        public async System.Threading.Tasks.Task Update(TeamMember teamMember)
        {
            await dal.Update(teamMember);
        }

        public async System.Threading.Tasks.Task Delete(TeamMember teamMember)
        {
            await dal.Delete(teamMember);
        }

        public async Task<List<CustomUser>> GetUsersByProjectIdsAsync(List<int> projectIds)
        {
            var items = await dal.GetAll();
            return   items 
                .Where(pa => projectIds.Contains(pa.ProjectId))
                .Select(pa => pa.User)  
                .ToList();
        }

    }
}
