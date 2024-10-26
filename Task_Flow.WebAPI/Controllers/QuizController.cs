using Microsoft.AspNetCore.Mvc;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        // GET: api/<QuizController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _quizService.Quizzes();
            var items = list.Select(l =>
            {
                return new QuizDto
                {
                    AgeRange = l.AgeRange,
                    Profession = l.Profession,
                    UsagePurpose = l.UsagePurpose, 
                };
            });
            return Ok(items);
        }

     
        // put api/<QuizController>
        [HttpPut("Profession")]
        public async Task<IActionResult> PutProfession([FromBody] string value)
        {
            var items=await _quizService.Quizzes();
           var last= items.Last();
          
            last.Profession = value;    
            await _quizService.Update(last);
            return Ok();
        }

        [HttpPut("Occupation")]
        public async Task<IActionResult> PutOccupation([FromBody] string value)
        {
            var items = await _quizService.Quizzes();
            var last = items.Last();

            last.UsagePurpose = value;
            await _quizService.Update(last);
            return Ok();
        }

    }
}
