using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Abstract
{
    public interface IUserService
    {
        Task<List<CustomUser>> GetUsers();
        Task<CustomUser> GetUserById(string id);
        Task<List<CustomUser>> GetUserByName(string name);
       Task Add(CustomUser user);
       Task Update(CustomUser user);
       Task Delete(CustomUser user);  
        Task<int> GetAllUserCount(); 
        Task<bool> CheckUsernameOrEmail(string nameOrEmail);
    }
}
