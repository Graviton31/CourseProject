using ElectronicJournalsApi.Data;
using ElectronicJournalsApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly ElectronicJournalContext _context;

        public SchedulesController(ElectronicJournalContext context)
        {
            _context = context;
        }

        // GET: api/schedules/group/{groupId}
        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetScheduleByGroup(int groupId)
        {
            var schedules = await _context.Schedules
                .Where(s => s.IdGroup == groupId)
                .ToListAsync();

            // Вместо NotFound возвращаем пустой список
            return Ok(schedules);
        }

        // POST: api/schedules/create
        [HttpPost("create")]
        public async Task<ActionResult<Schedule>> CreateScheduleEntry(ScheduleDto scheduleDto)
        {
            Console.WriteLine("\n\n\n\n\n\nРасписание");
            if (scheduleDto == null)
            {
                return BadRequest("Некорректные данные.");
            }

            // Валидация данных
            if (scheduleDto.WeekDay == null ||
                scheduleDto.StartTime == default ||
                scheduleDto.EndTime == default ||
                scheduleDto.StartTime >= scheduleDto.EndTime)
            {
                return BadRequest("Некорректные данные расписания.");
            }

            // Преобразование DTO в сущность Schedule
            var schedule = new Schedule
            {
                WeekDay = scheduleDto.WeekDay,
                StartTime = scheduleDto.StartTime,
                EndTime = scheduleDto.EndTime,
                IdGroup = scheduleDto.IdGroup
            };

            // Добавляем запись в контекст
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            // Возвращаем созданный объект
            return CreatedAtAction(nameof(GetScheduleByGroup), new { groupId = schedule.IdGroup }, schedule);
        }

        // DELETE: api/schedules/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            // Найти запись расписания по ID
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound(); // Если запись не найдена, вернуть 404
            }

            _context.Schedules.Remove(schedule); // Удалить запись
            await _context.SaveChangesAsync(); // Сохранить изменения в базе данных

            return Ok(new { message = "Запись успешно удалена." }); // Вернуть успешный ответ
        }

        // GET: api/schedules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetSchedules()
        {
            var schedules = await _context.Schedules
                .Include(s => s.IdGroupNavigation) // Включаем связанную сущность Group
                .Include(s => s.IdGroupNavigation.IdSubjectNavigation) // Включаем связанную сущность Subject
                .Select(s => new
                {
                    s.WeekDay,
                    s.StartTime,
                    s.EndTime,
                    s.IdGroup,
                    GroupName = s.IdGroupNavigation.Name, // Получаем только имя группы
                    Classroom = s.IdGroupNavigation.Classroom, // Получаем только класс
                    SubjectName = s.IdGroupNavigation.IdSubjectNavigation.Name // Получаем название предмета
                })
                .ToListAsync();

            // Выводим данные для проверки
            Console.WriteLine("Полученные расписания:");
            foreach (var schedule in schedules)
            {
                Console.WriteLine($"WeekDay: {schedule.WeekDay}, StartTime: {schedule.StartTime}, EndTime: {schedule.EndTime}, IdGroup: {schedule.IdGroup}, GroupName: {schedule.GroupName ?? "Нет данных"}, Classroom: {schedule.Classroom ?? "Нет данных"}, SubjectName: {schedule.SubjectName ?? "Нет данных"}");
            }

            return Ok(schedules);
        }

        public class ScheduleDto
        {
            public sbyte? WeekDay { get; set; }
            public TimeOnly StartTime { get; set; }
            public TimeOnly EndTime { get; set; }
            public int IdGroup { get; set; }
        }
    }
}
