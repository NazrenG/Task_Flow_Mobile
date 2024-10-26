using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUserDal dal;

        public UserService(IUserDal dal)
        {
            this.dal = dal;
        }

        public async Task Add(CustomUser user)
        {
            await dal.Add(user);
        }

        public async Task Delete(CustomUser user)
        {
            await dal.Delete(user);
        }

        public async Task<CustomUser> GetUserById(string id)
        {
            return await dal.GetById(f => f.Id == id);
        }

        public async Task<List<CustomUser>> GetUsers()
        {
            return await dal.GetAll();
        }
 

        public async Task Update(CustomUser user)
        {
            await dal.Update(user);
        }

        public async Task<bool> UserExists(string username)
        {
            var hasExist = await dal.GetById(f => f.UserName == username);
            return hasExist != null ? true : false;
        }

        public async Task<int> GetAllUserCount()
        {
            var list = await dal.GetAll();
            return list.Count;
        }


       
    }
}