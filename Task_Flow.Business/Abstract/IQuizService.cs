using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Abstract
{
    public interface IQuizService
    {
        Task Add(Quiz quiz);    
        Task Update(Quiz quiz); 
        Task<List<Quiz>> Quizzes(); 
    }
}
