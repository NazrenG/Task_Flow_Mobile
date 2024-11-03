using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Concrete
{
    public class QuizService : IQuizService
    {
        private readonly IQuizDal dal;

        public QuizService(IQuizDal dal)
        {
            this.dal = dal;
        }

        public async Task Add(Quiz quiz)
        {
            await dal.Add(quiz);    
        }

        

        public async Task<List<Quiz>> Quizzes()
        {
            return await dal.GetAll();
        }

        public async Task<int> SpecialOccupationCount(string occupation)
        {

            var list = await dal.GetAll();
          return list.Where(p=>p.UsagePurpose == occupation).ToList().Count();
        }

        public async Task Update(Quiz quiz)
        {
            await  dal.Update(quiz);
        }
    }
}
