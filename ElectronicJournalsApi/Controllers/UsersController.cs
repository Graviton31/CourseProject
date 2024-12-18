using ElectronicJournalApi.Classes;
using ElectronicJournalsApi.Data;
using ElectronicJournalsApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ElectronicJournalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ElectronicJournalContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(ElectronicJournalContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.Where(u => !u.IsDelete).ToListAsync();
        }

        // GET: api/teacher
        [HttpGet("teachers")]
        public async Task<ActionResult<IEnumerable<User>>> GetTeachers()
        {
            return await _context.Users.Where(u => !u.IsDelete && u.Role == "учитель").ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.IdUsers)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost("PostUser")]
        public async Task<ActionResult<User>> PostUser([FromBody] UserDto userDto)
        {
            Console.WriteLine($"Received UserDto: {JsonConvert.SerializeObject(userDto)}");

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == userDto.Login);
            if (existingUser != null)
            {
                return BadRequest(new { Message = "Пользователь с таким логином уже существует." });
            }

            try
            {
                Console.WriteLine(userDto.PasswordString);
                Console.WriteLine(userDto.Role);
                Console.WriteLine(userDto.Login);
                // Создаем нового пользователя
                var newUser = new User
                {
                    Surname = userDto.Surname,
                    Name = userDto.Name,
                    Patronymic = userDto.Patronymic,
                    Login = userDto.Login,
                    Password = PasswordHasher.HashPassword(userDto.PasswordString, userDto.Login), // Хешируем пароль
                    Phone = userDto.Phone,
                    BirthDate = userDto.BirthDate,
                    Role = userDto.Role
                };

                // Добавляем пользователя в контекст
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new { User = userDto.Login, Message = "Пользователь успешно создан." });
            }
            catch (DbUpdateException dbEx)
            {
                Console.Error.WriteLine($"Database update error: {dbEx.Message}");
                return StatusCode(500, new { Message = $"Ошибка при сохранении данных пользователя. Пожалуйста, попробуйте еще раз.", InnerException = dbEx.InnerException?.Message });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, new { Message = "Произошла непредвиденная ошибка. Пожалуйста, попробуйте еще раз." });
            }

        }

        // DELETE: api/Users/5
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Устанавливаем IsDelete в true
            user.IsDelete = true;

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();

            return Ok(new { message = "Пользователь успешно удален" }); // Возвращаем статус 200 OK с сообщением
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.IdUsers == id);
        }

        // GET: api/Users/roles
        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetRoles()
        {
            var roles = await _context.Users
                .Select(u => u.Role)
                .Distinct()
                .ToListAsync();

            return Ok(roles);
        }

        [HttpPost("Authorization")]
        public IActionResult Authorization([FromBody] UserAuthorizationDto userAuthorizationDto)
        {
            Console.WriteLine("\n\n\n\nAuthorization RUN\n\n\n\n");

            // Поиск пользователя в базе данных по логину
            var existingUser = _context.Users.SingleOrDefault(u => u.Login == userAuthorizationDto.Login);
            if (existingUser == null)
            {
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }

            // Проверка пароля
            if (PasswordHasher.VerifyPassword(userAuthorizationDto.Password, userAuthorizationDto.Login, existingUser.Password))
            {
                Console.WriteLine(existingUser.Role);

                // Генерация JWT токена
                var token = GenerateJwtToken(existingUser);

                // Возвращаем токен и роль пользователя
                return Ok(new
                {
                    message = "Успешная авторизация",
                    token = token,
                    role = existingUser.Role, // Предполагается, что у вас есть поле Role в модели User
                    login = existingUser.Login
                });
            }

            return Unauthorized(new { message = "Неверный логин или пароль" });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.NameId, user.IdUsers.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), // Установите время жизни токена
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public class UserAuthorizationDto
        {
            public string Login { get; set; }
            public string Password { get; set; }

        }

        public partial class UserDto
        {
            public string? Surname { get; set; }

            public string? Name { get; set; }

            public string? Patronymic { get; set; }

            public string Login { get; set; } = null!;

            public string PasswordString { get; set; } = null!;

            public string? Phone { get; set; }

            public DateOnly? BirthDate { get; set; }

            public string Role { get; set; } = null!;
        }
    }
}
