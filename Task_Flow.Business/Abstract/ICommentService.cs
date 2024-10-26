using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Abstract
{
    public interface ICommentService
    {
        Task<List<Comment>> GetComments();
        Task<Comment> GetCommentById(int id);
        Task Add(Comment comment);
        Task Update(Comment comment);
        Task Delete(Comment comment);
        Task<int> GetCount();

    }
}
