using ElectronicJournalsApi.Data;
using ElectronicJournalsApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnvisitedStatusesController : ControllerBase
    {
        private readonly ElectronicJournalContext _context;

        public UnvisitedStatusesController(ElectronicJournalContext context)
        {
            _context = context;
        }

        // GET: api/unvisitedstatus
        [HttpGet]
        public ActionResult<IEnumerable<UnvisitedStatus>> GetUnvisitedStatuses()
        {
            // Получаем все статусы из базы данных
            var statuses = _context.UnvisitedStatuses.ToList();

            // Проверяем, есть ли статусы
            if (statuses == null || !statuses.Any())
            {
                return NotFound(); // Возвращаем 404, если статусы не найдены
            }
            return Ok(statuses); // Возвращаем статусы с кодом 200
        }
    }
}
