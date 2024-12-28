using ElectronicJournalsApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace ElectronicJournalApp.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly HttpClient _httpClient;

        public SubjectsController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _httpClient.GetFromJsonAsync<IEnumerable<Subject>>("https://localhost:7022/api/subjects");
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Получаем предмет по ID
                var subject = await _httpClient.GetFromJsonAsync<Subject>($"https://localhost:7022/api/subjects/{id}");

                if (subject == null)
                {
                    return NotFound(); // Если предмет не найден, возвращаем 404
                }

                return View(subject); // Передаем предмет в представление
            }
            catch (HttpRequestException ex)
            {
                // Логирование ошибки
                Console.Error.WriteLine($"Error fetching subject: {ex.Message}");
                return View("Error"); // Показать страницу ошибки
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            // Получаем предмет по ID
            var subjectResponse = await _httpClient.GetAsync($"https://localhost:7022/api/Subjects/{id}");
            if (!subjectResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var subject = await subjectResponse.Content.ReadFromJsonAsync<Subject>();

            // Получаем группы по ID предмета
            var groupsResponse = await _httpClient.GetAsync($"https://localhost:7022/api/Subjects/{id}/Groups");
            if (groupsResponse.IsSuccessStatusCode)
            {
                var groups = await groupsResponse.Content.ReadFromJsonAsync<IEnumerable<Group>>();

                // Загружаем информацию о пользователе для каждой группы
                foreach (var group in groups)
                {
                    var userResponse = await _httpClient.GetAsync($"https://localhost:7022/api/Users/{group.IdUsers}");
                    if (userResponse.IsSuccessStatusCode)
                    {
                        var user = await userResponse.Content.ReadFromJsonAsync<User>();
                        group.IdUsersNavigation = user; // Присваиваем пользователя группе
                    }
                }

                subject.Groups = groups.ToList(); // Предполагается, что у вас есть свойство Groups в модели Subject
            }

            return View(subject);
        }

    }
}
