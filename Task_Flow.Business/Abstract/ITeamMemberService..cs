using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Abstract
{
    public interface ITeamMemberService
    {
        Task<List<TeamMember>> TeamMembers();
        Task<TeamMember> GetTaskMemberById(int id);
        Task<List<TeamMember>>GetProjectListByUserIdAsync(string id);
     Task Add(TeamMember teamMember);
        Task<List<TeamMember>> GetTaskMemberListById(int id);
        Task RemoveMembers(List<TeamMember>members);
        Task DeleteTeamMemberAsync(int projectId, string teamMemberId);
     Task Update(TeamMember teamMember);
     Task Delete(TeamMember teamMember);
        Task<List<CustomUser>> GetUsersByProjectIdsAsync(List<int> projectIds);
    }
}
