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
        private readonly static List<string> OccupationList = new List<string>
        {
             "IT (Programming, Systems)",
          "Design (Graphic, UI/UX)", 
          "Human Resources",
          "Software Programming",
          "Backend Developer",
          "Frontend Developer",
          "Other (please specify)"
        };
        private readonly static List<string> ProfessionList = new List<string>
        {
            "Programming",
          "Marketing",
          "Accounting",
          "Education",
          "Other (please specify)"
        };
        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        //occupation statistic
        [HttpGet("OccupationStatistic")]
        public async Task<IActionResult> GetOccupationStatistic()
        {
            var list = await _quizService.Quizzes();
            var totalCount = list.Count;

            var occupationStatistics = new List<OccupationStatisticDto>();

            foreach (var item in OccupationList)
            {
                var count = await _quizService.SpecialOccupationCount(item);
                var percentage = totalCount > 0 ? Math.Round(count * 100m / totalCount, 2) : 0; 
                occupationStatistics.Add(new OccupationStatisticDto
                {
                    OccupationName = item,
                    Percentage = percentage
                });
            }

            return Ok(occupationStatistics);
        }

        //profession statistic
        [HttpGet("ProfessionStatistic")]
        public async Task<IActionResult> GetProfessionStatistic()
        {
            var list = await _quizService.Quizzes();
            var totalCount = list.Count;

            var professionStatistics = new List<OccupationStatisticDto>();

            foreach (var item in OccupationList)
            {
                var count = await _quizService.SpecialOccupationCount(item);
                var percentage = totalCount > 0 ? Math.Round(count * 100m / totalCount, 2) : 0;
                professionStatistics.Add(new OccupationStatisticDto
                {
                    OccupationName = item,
                    Percentage = percentage
                });
            }

            return Ok(professionStatistics);
        }


        // put api/<QuizController>
        [HttpPut("Profession")]
        public async Task<IActionResult> PutProfession([FromBody] string value)
        {
            var items = await _quizService.Quizzes();
            var last = items.Last();

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
