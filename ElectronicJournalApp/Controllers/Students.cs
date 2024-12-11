using Microsoft.AspNetCore.Mvc;

namespace ElectronicJournalApp.Controllers
{
    public class Students : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
