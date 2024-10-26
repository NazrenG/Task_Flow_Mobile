using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Abstract
{
    public interface ITeamMemberService
    {
        Task<List<TeamMember>> TeamMembers();
        Task<TeamMember> GetTaskMemberById(int id);
     Task Add(TeamMember teamMember);
     Task Update(TeamMember teamMember);
     Task Delete(TeamMember teamMember);
    }
}
