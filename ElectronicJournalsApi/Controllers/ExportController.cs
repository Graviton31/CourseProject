using ClosedXML.Excel;
using ElectronicJournalsApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly ElectronicJournalContext _context;

        public ExportController(ElectronicJournalContext context)
        {
            _context = context;
        }

        [HttpGet("journalsExport/{id}")]
        public IActionResult ExportAttendanceData(int id)
        {
            var group = _context.Groups
                .Include(g => g.IdStudents)
                    .ThenInclude(s => s.Journals)
                        .ThenInclude(j => j.IdUnvisitedStatusNavigation)
                .Include(g => g.IdSubjectNavigation)
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
                    .OrderBy(j => j.LessonDate)
                    .Select(j => new
                    {
                        j.LessonDate,
                        StatusShortName = j.IdUnvisitedStatusNavigation != null ? j.IdUnvisitedStatusNavigation.ShortName : "+",
                    })
                    .ToList()
            }).ToList();

            // Создание Excel файла
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Attendance Data");

                // Заголовки группы и предмета
                worksheet.Cell(1, 1).Value = $"Предмет: {group.IdSubjectNavigation.Name}";
                worksheet.Cell(1, 2).Value = $"Группа: {group.Name}";

                // Заголовки таблицы
                worksheet.Cell(4, 1).Value = "№";
                worksheet.Cell(4, 2).Value = "ФИО";

                // Получение уникальных дат уроков
                var uniqueDates = students.SelectMany(s => s.Journals)
                    .Select(j => j.LessonDate)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                // Заполнение заголовков дат
                for (int i = 0; i < uniqueDates.Count; i++)
                {
                    var dateCell = worksheet.Cell(4, 3 + i);
                    dateCell.Value = uniqueDates[i]?.ToString("MM/dd");
                    dateCell.Style.Fill.BackgroundColor = XLColor.LightYellow; // Цвет для ячеек с датами
                }

                // Заголовки для новых столбцов
                int startColumnForNewData = 3 + uniqueDates.Count; // Начинаем с первого столбца после дат
                worksheet.Cell(4, startColumnForNewData+1).Value = "Прогулы";
                worksheet.Cell(4, startColumnForNewData + 2).Value = "Пропуски";
                worksheet.Cell(4, startColumnForNewData + 3).Value = "Прогулы %";

                int row = 5; // Начинаем с 5-й строки
                foreach (var student in students)
                {
                    worksheet.Cell(row, 1).Value = row - 4; // Номер строки
                    worksheet.Cell(row, 2).Value = $"{student.Surname} {student.Name}";

                    // Выделение ячейки с именем студента
                    worksheet.Cell(row, 2).Style.Fill.BackgroundColor = XLColor.Yellow; // Цвет для ячеек с именами студентов

                    // Заполнение статусов по датам
                    for (int i = 0; i < uniqueDates.Count; i++)
                    {
                        var journal = student.Journals.FirstOrDefault(j => j.LessonDate == uniqueDates[i]);
                        string status = journal != null ? journal.StatusShortName : string.Empty;
                        worksheet.Cell(row, 3 + i).Value = status;
                    }

                    // Выделение ячейки с номером строки
                    worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightYellow1; // Цвет для ячеек с номерами
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "attendance_data.xlsx");
                }
            }
        }
    }
}

