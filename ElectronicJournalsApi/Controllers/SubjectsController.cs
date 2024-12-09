using ElectronicJournalsApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ElectronicJournalContext _context;

        public SubjectsController(ElectronicJournalContext context)
        {
            _context = context;
        }

        [HttpGet("teacher/{login}")]
        public IActionResult GetSubjectsByTeacher(string login)
        {
            var teacher = _context.Users.FirstOrDefault(u => u.Login == login);
            if (teacher == null)
            {
                return NotFound(); // Учитель не найден
            }

            var subjectsWithGroups = _context.Subjects
                .Include(s => s.Groups)
                .Where(s => s.IdUsers.Any(u => u.IdUsers == teacher.IdUsers))
                .Select(s => new
                {
                    s.Name,
                    s.Description,
                    Groups = s.Groups.Select(g => new
                    {
                        g.Name,
                        g.IdGroup // Добавляем IdGroup в выборку
                    })
                })
                .ToList();

            return Ok(subjectsWithGroups);
        }

		[HttpGet("group/{id}")]
		public IActionResult GetStudentsByGroup(int id, int pageNumber = 1, int pageSize = 10)
		{
			var group = _context.Groups
				.Include(g => g.IdStudents)
					.ThenInclude(s => s.Journals)
						.ThenInclude(j => j.IdUnvisitedStatusNavigation)
				.FirstOrDefault(g => g.IdGroup == id);

			if (group == null)
			{
				return NotFound(); // Группа не найдена
			}

			var students = group.IdStudents.Select(s => new
			{
				s.IdStudent,
				s.Name,
				s.Surname,
				Journals = s.Journals
					.Where(j => j.IdGroup == id)
					.OrderBy(j => j.LessonDate) // Сортировка по возрастанию даты
					.Select(j => new
					{
						j.LessonDate,
						StatusShortName = j.IdUnvisitedStatusNavigation != null ? j.IdUnvisitedStatusNavigation.ShortName : "+", // Проверяем на null
						StatusId = j.IdUnvisitedStatus != null ? j.IdUnvisitedStatus : (int?)null // Добавляем ID статуса
					})
					.ToList()
			}).ToList();

			// Пагинация студентов
			var totalCount = students.Count(); // Общее количество студентов
			var paginatedStudents = students.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(); // Пагинация

			// Получаем уникальные даты уроков
			var lessonDates = paginatedStudents.SelectMany(s => s.Journals.Select(j => j.LessonDate)).Distinct().ToList();

			return Ok(new
			{
				Students = paginatedStudents,
				LessonDates = lessonDates,
				TotalCount = totalCount,
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize) // Общее количество страниц
			});
		}
	}
}
