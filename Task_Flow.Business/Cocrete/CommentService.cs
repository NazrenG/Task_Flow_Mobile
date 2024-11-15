using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class CommentService : ICommentService
    {
        private readonly ICommentDal dal;

        public CommentService(ICommentDal dal)
        {
            this.dal = dal;
        }

        public async  Task Add(Comment comment)
        {
           await dal.Add(comment);
        }

        public async Task Delete(Comment comment)
        {
          await dal.Delete(comment);
        }

        public async Task<Comment> GetCommentById(int id)
        {
          return await dal.GetById(f=>f.Id == id);
        }

        public async Task<List<Comment>> GetComments()
        {
           return await dal.GetAll();
        }

        public async Task<int> GetCount()
        {
            var list= await dal.GetAll();   
            return list.Count;
        }

        public async  Task Update(Comment address)
        {
            await dal.Update(address);  
        }
    }
}
