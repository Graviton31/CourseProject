using Microsoft.AspNetCore.Mvc;

namespace ElectronicJournalApp.Controllers
{
    public class SchedulesController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }
    }
}
