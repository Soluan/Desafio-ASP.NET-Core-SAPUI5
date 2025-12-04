using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using MyApp.Services;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _svc;
        public UsersController(IUserService svc) { _svc = svc; }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string? title = null, [FromQuery] string? sort = "id", [FromQuery] string? order = "asc")
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);
            var (items, total) = await _svc.GetUsersAsync(page, pageSize, title, sort, order);
            var result = new { page, pageSize, total, items };
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var todo = await _svc.GetByIdAsync(id);
            if (todo == null) return NotFound();
            return Ok(todo);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCompleted(int id, [FromBody] UserDto dto)
        {
            try
            {
                var ok = await _svc.UpdateCompletedAsync(id, dto.Completed);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync()
        {
            var result = await _svc.SyncFromExternalAsync();
            return Ok(new { added = result.added, skipped = result.skipped, messages = result.messages });
        }
    }
}