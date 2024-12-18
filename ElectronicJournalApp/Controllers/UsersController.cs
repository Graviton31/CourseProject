using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ElectronicJournalsApi.Data;
using ElectronicJournalsApi.Models;
using System.Net.Http;

namespace ElectronicJournalApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly HttpClient _httpClient;

        public UsersController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _httpClient.GetFromJsonAsync<IEnumerable<User>>("https://localhost:7022/api/users");
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Authorization()
        {
            return View();
        }

        // Здесь вы можете добавить методы для создания, редактирования и удаления пользователей
    }
}
