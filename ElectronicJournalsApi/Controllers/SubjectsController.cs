﻿using ElectronicJournalsApi.Data;
using ElectronicJournalsApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;

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

        [HttpGet("{id}/Groups")]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroupsBySubjectId(int id)
        {
            var groups = await _context.Groups
                .Where(g => g.IdSubject == id)
                .ToListAsync();

            if (groups == null || !groups.Any())
            {
                return NotFound(new { Message = "Группы не найдены." });
            }

            return Ok(groups);
        }

        [HttpPut("UpdateSubject")]
        public async Task<ActionResult<Subject>> UpdateSubject([FromBody] SubjectUpdateDto subjectUpdateDto)
        {
            Console.WriteLine($"\n\n\n\nReceived Subject Update: {JsonConvert.SerializeObject(subjectUpdateDto)}");

            if (!ModelState.IsValid)
            {
                var errorMessages = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                Console.Error.WriteLine($"\n\n\n\n\nValidation errors: {errorMessages}");
                return BadRequest(new { Message = "Ошибка в данных запроса.", Errors = errorMessages });
            }

            try
            {
                // Находим предмет по Id
                var subject = await _context.Subjects
                    .Include(s => s.Groups)
                    .FirstOrDefaultAsync(s => s.IdSubject == subjectUpdateDto.IdSubject);

                if (subject == null)
                {
                    return NotFound(new { Message = "Предмет не найден." });
                }

                // Обновляем данные предмета
                subject.Name = subjectUpdateDto.Name;
                subject.FullName = subjectUpdateDto.FullName;
                subject.Description = subjectUpdateDto.Description;
                subject.Duration = subjectUpdateDto.Duration;
                subject.LessonLength = subjectUpdateDto.LessonLength;
                subject.LessonsCount = subjectUpdateDto.LessonsCount;

                // Обновляем группы
                var existingGroupIds = subject.Groups.Select(g => g.IdGroup).ToList();
                var groupsToUpdate = subjectUpdateDto.Groups.Where(g => g.IdGroup > 0).ToList(); // Группы для обновления
                var groupsToAdd = subjectUpdateDto.Groups.Where(g => g.IdGroup <= 0).ToList(); // Группы для добавления

                // Обновляем существующие группы
                foreach (var updatedGroup in groupsToUpdate)
                {
                    var existingGroup = subject.Groups.FirstOrDefault(g => g.IdGroup == updatedGroup.IdGroup);
                    if (existingGroup != null)
                    {
                        existingGroup.Name = updatedGroup.Name;
                        existingGroup.StudentCount = updatedGroup.StudentCount;
                        existingGroup.Classroom = updatedGroup.Classroom;
                        existingGroup.IdUsers = updatedGroup.IdUsers;
                    }
                }

                // Добавляем новые группы
                foreach (var group in groupsToAdd)
                {
                    var newGroup = new Group
                    {
                        Name = group.Name,
                        StudentCount = group.StudentCount,
                        Classroom = group.Classroom,
                        IdUsers = group.IdUsers,
                        IdSubject = subject.IdSubject
                    };
                    subject.Groups.Add(newGroup);
                }


                await _context.SaveChangesAsync();

                return Ok(new { Subject = subject.Name, Message = "Предмет успешно обновлен." });
            }
            catch (DbUpdateException dbEx)
            {
                Console.Error.WriteLine($"Database update error: {dbEx.Message}");
                return StatusCode(500, new { Message = "Ошибка при сохранении данных предмета. Пожалуйста, попробуйте еще раз.", InnerException = dbEx.InnerException?.Message });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, new { Message = "Произошла непредвиденная ошибка. Пожалуйста, попробуйте еще раз." });
            }
        }

        // GET: api/Subjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subject>>> GetSubjects()
        {
            return await _context.Subjects.Where(j => !j.IsDelete).ToListAsync();
        }

        // GET: api/Subjects/subjectsGrops
        [HttpGet("subjectsGroups")]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjectsGroups()
        {
            // Получаем предметы с группами, фильтруя удаленные предметы
            var subjectsWithGroups = await _context.Subjects
                .Where(s => !s.IsDelete) // Фильтрация предметов, которые не помечены как удаленные
                .Select(s => new SubjectDto
                {
                    Name = s.Name, // Получаем имя предмета
                    Groups = s.Groups.Select(g => new GroupDto
                    {
                        Name = g.Name, // Получаем имя группы
                        IdGroup = g.IdGroup // Получаем идентификатор группы
                    }).ToList() // Преобразуем группы в список DTO
                })
                .ToListAsync(); // Асинхронно выполняем запрос и получаем список предметов

            // Проверяем, найдены ли предметы
            if (!subjectsWithGroups.Any())
            {
                return NotFound(); // Возвращаем 404, если предметы не найдены
            }

            return Ok(subjectsWithGroups); // Возвращаем 200 и список найденных предметов
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> GetSubjectById(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
            {
                return NotFound(new { Message = "Предмет не найден." });
            }

            return Ok(subject);
        }

        [HttpGet("teacher/{login}")]
        public async Task<IActionResult> GetSubjectsByTeacher(string login)
        {
            // Получаем предметы, связанные с учителем по логину
            var subjectsWithGroups = await _context.Subjects
                .Include(s => s.Groups)
                .Where(s => s.IdUsers.Any(u => u.Login == login) && !s.IsDelete) // Фильтрация по IsDelete
                .Select(s => new SubjectDto
                {
                    Name = s.Name,
                    Groups = s.Groups.Select(g => new GroupDto
                    {
                        Name = g.Name,
                        IdGroup = g.IdGroup
                    }).ToList()
                })
                .ToListAsync();

            if (!subjectsWithGroups.Any())
            {
                return NotFound(); // Если предметы не найдены
            }

            return Ok(subjectsWithGroups);
        }

        [HttpGet("group/{id}")]
        public IActionResult GetStudentsByGroup(int id)
        {
            var group = _context.Groups
                .Include(g => g.IdStudents)
                    .ThenInclude(s => s.Journals)
                        .ThenInclude(j => j.IdUnvisitedStatusNavigation)
                .Include(g => g.IdSubjectNavigation) // Добавляем включение для предмета
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

            // Получаем уникальные даты уроков
            var lessonDates = students.SelectMany(s => s.Journals.Select(j => j.LessonDate)).Distinct().ToList();

            // Создаем объект ответа, включая имя группы и имя предмета
            return Ok(new
            {
                GroupName = group.Name, // Имя группы
                SubjectName = group.IdSubjectNavigation.Name, // Имя предмета
                Students = students,
                LessonDates = lessonDates,
            });
        }

        [HttpPost("PostSubject")]
        public async Task<ActionResult<Subject>> PostSubject([FromBody] Subject subject)
        {
            Console.WriteLine($"Received Subject: {JsonConvert.SerializeObject(subject)}");

            try
            {
                // Добавляем предмет в контекст
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();

                return Ok(new { Subject = subject.Name, Message = "Предмет успешно создан." });
            }
            catch (DbUpdateException dbEx)
            {
                Console.Error.WriteLine($"Database update error: {dbEx.Message}");
                return StatusCode(500, new { Message = "Ошибка при сохранении данных предмета. Пожалуйста, попробуйте еще раз.", InnerException = dbEx.InnerException?.Message });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, new { Message = "Произошла непредвиденная ошибка. Пожалуйста, попробуйте еще раз." });
            }
        }

        // DELETE: api/Subjects/DeleteSubject/{id}
        [HttpDelete("DeleteSubject/{id}")]
        public async Task<IActionResult> DeleteSubject([FromRoute] int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            // Устанавливаем IsDelete в true
            subject.IsDelete = true;

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();

            return Ok(new { message = "Предмет успешно удален" }); // Возвращаем статус 200 OK с сообщением
        }

        // DTO классы
        public class SubjectDto
        {
            public string Name { get; set; }
            public string? Description { get; set; }
            public List<GroupDto> Groups { get; set; }
        }

        public class GroupDto
        {
            public string Name { get; set; }
            public int IdGroup { get; set; }
        }

        public class SubjectUpdateDto
        {
            public int IdSubject { get; set; }
            public string Name { get; set; }
            public string FullName { get; set; }
            public string? Description { get; set; }
            public sbyte Duration { get; set; }
            public sbyte LessonLength { get; set; }
            public sbyte LessonsCount { get; set; }
            public List<GroupUpdateDto>? Groups { get; set; } = new List<GroupUpdateDto>();
        }

        public class GroupUpdateDto
        {
            public int? IdGroup { get; set; } 

            public string Name { get; set; } = null!;

            public sbyte? StudentCount { get; set; }

            public string Classroom { get; set; } = null!;

            public int IdUsers { get; set; }
        }
    }
}
