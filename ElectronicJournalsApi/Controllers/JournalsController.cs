using ElectronicJournalsApi.Data;
using ElectronicJournalsApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JournalsController : ControllerBase
    {
        private readonly ElectronicJournalContext _context;

        public JournalsController(ElectronicJournalContext context)
        {
            _context = context;
        }

        [HttpPost("CreateJournal")]
        public IActionResult CreateJournalEntries([FromBody] List<JournalDto> journalDtos)
        {
            Console.WriteLine("\n\n\n\n\n\n CreateJournal");
            // Проверяем, что данные не пустые
            if (journalDtos == null || !journalDtos.Any())
            {
                return BadRequest("Нет данных для сохранения.");
            }

            var journalEntries = new List<Journal>();

            foreach (var dto in journalDtos)
            {
                // Проверяем, существует ли запись с такими же данными
                var existingEntry = _context.Journals
                    .FirstOrDefault(j => j.LessonDate == dto.LessonDate &&
                                         j.IdGroup == dto.IdGroup &&
                                         j.IdStudent == dto.IdStudent);

                if (existingEntry != null)
                {
                    // Если запись существует, обновляем её
                    existingEntry.IdUnvisitedStatus = dto.IdUnvisitedStatus;
                    _context.Journals.Update(existingEntry); // Обновляем запись
                }
                else
                {
                    // Если записи нет, создаем новую
                    var entry = new Journal
                    {
                        LessonDate = dto.LessonDate,
                        IdGroup = dto.IdGroup,
                        IdStudent = dto.IdStudent,
                        IdUnvisitedStatus = dto.IdUnvisitedStatus
                    };
                    journalEntries.Add(entry); // Добавьте запись в список
                }
            }

            // Сохраняем изменения для новых записей
            if (journalEntries.Any())
            {
                const int batchSize = 30; // Размер пакета
                for (int i = 0; i < journalEntries.Count; i += batchSize)
                {
                    var batch = journalEntries.Skip(i).Take(batchSize).ToList();
                    _context.Journals.AddRange(batch);
                }
            }

            // Сохраняем все изменения в базе данных
            _context.SaveChanges();

            return Ok(journalEntries);
        }

        public class JournalDto
        {
            public DateOnly? LessonDate { get; set; }
            public int IdGroup { get; set; }
            public int IdStudent { get; set; }
            public int? IdUnvisitedStatus { get; set; }
        }
    }
}

