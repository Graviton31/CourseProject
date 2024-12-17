using Microsoft.AspNetCore.Mvc;

namespace ElectronicJournalApp.Controllers
{
    public class StudentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
