using ElectronicJournalsApi.Data;
using ElectronicJournalsApi.Models;
using System.Data;

namespace ElectronicJournalApi.Classes
{
    public class UserSeeder
    {
        public static void CreateDefaultUsers(ElectronicJournalContext context, IConfiguration configuration)
        {
            var defaultLoginAdminig = configuration.GetSection("DefaultAdmin");

            var defaultLoginTeacher = configuration.GetSection("DefaultTeacher");

            var defaultLoginManager = configuration.GetSection("DefaultManager");

            // Массив стандартных пользователей
            var defaultUsers = new List<User>
    {
        new User
        {
            Login = defaultLoginAdminig["Login"], // Стандартный логин
            Password = PasswordHasher.HashPassword(defaultLoginAdminig["Password"], defaultLoginAdminig["Login"]),
            Role = "Администратор" // Роль для стандартного пользователя
        },
        new User
        {
            Login = defaultLoginTeacher["Login"], // Логин для второго пользователя
            Password = PasswordHasher.HashPassword(defaultLoginTeacher["Password"], defaultLoginTeacher["Login"]),
            Role = "Учитель" // Роль для второго пользователя
        },
        new User
        {
            Login = defaultLoginManager["Login"], // Логин для третьего пользователя
            Password = PasswordHasher.HashPassword(defaultLoginManager["Password"], defaultLoginManager["Login"] ),
            Role = "Руководитель" // Роль для третьего пользователя
        }
    };

            // Добавление пользователей в контекст
            foreach (var user in defaultUsers)
            {
                if (!context.Users.Any(u => u.Login == user.Login)) // Проверка на существование
                {
                    context.Users.Add(user);
                }
            }

            context.SaveChanges(); // Сохранение изменений в базе данных
        }
    }
}
