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

    }
}
