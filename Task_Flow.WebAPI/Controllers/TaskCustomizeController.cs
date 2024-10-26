using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskCustomizeController : ControllerBase
    {
        private readonly ITaskCustomizeService _taskCustomizeService;

        public TaskCustomizeController(ITaskCustomizeService taskCustomizeService)
        {
            _taskCustomizeService = taskCustomizeService;
        }

        // GET: api/<TaskCustomizeController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var items = await _taskCustomizeService.GetCustomize();
            if (items == null)
            {
                return NotFound();
            }
            var list = items.Select(l =>
            {
                return new TaskCustomizeDto
                {
                    TagColor = l.TagColor,
                    BackColor = l.BackColor,
                    TaskId = l.TaskId
                };
            });
            return Ok(list);

        }
        [HttpGet("BackColors")]
        public async Task<IActionResult> GetBackColors()
        {
            var list = await _taskCustomizeService.GetCustomize();
            if (list == null) return NotFound();

            var backGroundColorList = list.Select(c => c.BackColor).Distinct().ToList();

            return Ok(backGroundColorList);
        }

        [HttpGet("TagColors")]
        public async Task<IActionResult> GetTagColors()
        {
            var list = await _taskCustomizeService.GetCustomize();
            if (list == null) return NotFound();

            var tagColorList = list.Select(c => c.TagColor).Distinct().ToList();

            return Ok(tagColorList);
        }

        // GET api/<TaskCustomizeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _taskCustomizeService.GetCustomizeById(id);
            if (item == null)
            {
                return NotFound();
            }
            var project = new TaskCustomizeDto
            {
                TagColor = item.TagColor,
                BackColor = item.BackColor,
                TaskId = item.TaskId
            };
            return Ok(project);

        }

        // POST api/<TaskCustomizeController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TaskCustomizeDto value)
        {
            var item = new TaskCustomize
            {
                TagColor = value.TagColor,
                BackColor = value.BackColor,
                TaskId = value.TaskId,
            };
            await _taskCustomizeService.Add(item);
            return Ok(item);
        }

        // PUT api/<TaskCustomizeController>/5
        [HttpPut("BackgroundColor/{id}")]
        public async Task<IActionResult> PutBackColor(int id, [FromBody] string value)
        {
            var item = await _taskCustomizeService.GetCustomizeById(id);
            if (item == null) { return NotFound(); }
            item.BackColor = value;
            await _taskCustomizeService.Update(item);
            return Ok();

        }
        [HttpPut("TagColor/{id}")]
        public async Task<IActionResult> PutTagColor(int id, [FromBody] string value)
        {
            var item = await _taskCustomizeService.GetCustomizeById(id);
            if (item == null) { return NotFound(); }
            item.TagColor = value;
            await _taskCustomizeService.Update(item);
            return Ok();

        }
        // DELETE api/<TaskCustomizeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _taskCustomizeService.GetCustomizeById(id);
            if (item == null) { return NotFound(); };
            await _taskCustomizeService.Delete(item);
            return Ok();
        }
    }
}
