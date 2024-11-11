using Microsoft.AspNetCore.Mvc;
using Task_Flow.DataAccess.Abstract; 
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
 
namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: api/<CommentController>
        [HttpGet("Commits")]
        public async Task<IActionResult> Get()
        {
            var list = await _commentService.GetComments();
            var items = list.Select(p =>
            {
                return new CommentDto
                {
                    Context = p.Context,
                    TaskForUserId = p.TaskForUserId,
                    UserId = p.UserId, 
                };
            });
            return Ok(items);
        }
        [HttpGet("CommitCount")]
        public async Task<IActionResult> GetCommintCount()
        {
            var count = await _commentService.GetCount();
           
            return Ok(count);
        }

        // GET api/<CommentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _commentService.GetCommentById(id);
            if (item == null)
            {
                return NotFound();
            }
            var comment = new CommentDto
            {
                Context = item.Context,
                TaskForUserId = item.TaskForUserId,
                UserId = item.UserId,
            };
            return Ok(comment);
        }

        // POST api/<CommentController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CommentDto value)
        {
            var item = new Comment
            {
                Context = value.Context,
                TaskForUserId = value.TaskForUserId,
                UserId = value.UserId, 
            };
            await _commentService.Add(item);
            return Ok(item);
        }

        // PUT api/<CommentController>/5
        [HttpPut("ChangeContext/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] string value)
        {
            var item = await _commentService.GetCommentById(id);
            if (item == null)
            {
                return NotFound();
            }
            item.Context = value;
            await _commentService.Update(item);
            return Ok();
        }

        // DELETE api/<CommentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _commentService.GetCommentById(id);
            if (item == null)
            {
                return NotFound();
            } 
            await _commentService.Delete(item);
            return Ok();
        }
    }
}
