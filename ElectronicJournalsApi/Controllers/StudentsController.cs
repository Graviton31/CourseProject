using ElectronicJournalsApi.Data;
using ElectronicJournalsApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly ElectronicJournalContext _context;

        public StudentsController(ElectronicJournalContext context)
        {
            _context = context;
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsByGroup(int groupId)
        {
            var students = await _context.Students
                .Where(s => s.IdGroups.Any(g => g.IdGroup == groupId))
                .OrderBy(s => s.Surname)
                .ToListAsync();

            return Ok(students);
        }
    }
}
