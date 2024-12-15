using ElectronicJournalApp.Models;
using ElectronicJournalsApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using static ElectronicJournalApi.Controllers.SchedulesController;

namespace ElectronicJournalApp.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly HttpClient _httpClient;

        public SchedulesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult ScheduleEdit()
        {
            ViewData["ActiveMenu"] = "ScheduleEdit";
            return View();
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ActiveMenu"] = "Index";
            var schedules = await _httpClient.GetFromJsonAsync<IEnumerable<ScheduleView>>("https://localhost:7022/api/schedules");
            foreach (var dto in schedules)
            {
                Console.WriteLine($"WeekDay: {dto.WeekDay}, StartTime: {dto.StartTime}, EndTime: {dto.EndTime}, IdGroup: {dto.IdGroup}, GroupName: {dto.GroupName}, Classroom: {dto.Classroom},");
            }
            return View(schedules);
        }
    }
}
