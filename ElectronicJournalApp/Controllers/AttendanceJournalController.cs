using Microsoft.AspNetCore.Mvc;

namespace ElectronicJournalApp.Controllers
{
    public class AttendanceJournalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
