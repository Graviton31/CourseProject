using System.Security.Cryptography;
using System.Text;

namespace ElectronicJournalApi.Classes
{
    public class PasswordHasher
    {
        public static string GenerateSaltedPassword(string password, string username)
        {
            var saltedPassword = new StringBuilder();
            int maxLength = Math.Max(password.Length, username.Length);

            for (int i = 0; i < maxLength; i++)
            {
                if (i < username.Length)
                {
                    saltedPassword.Append(username[i]);
                }
                if (i < password.Length)
                {
                    saltedPassword.Append(password[i]);
                }
            }

            return saltedPassword.ToString();
        }

        public static string HashPassword(string password, string username)
        {
            // Генерируем соль
            string saltedPassword = GenerateSaltedPassword(password, username);

            // Хешируем с помощью SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Преобразуем в шестнадцатеричное представление
                }
                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string enteredPassword, string username, string storedHash)
        {
            // Хешируем введенный пароль с использованием того же логина
            string hashedEnteredPassword = HashPassword(enteredPassword, username);

            // Сравниваем хеши
            return hashedEnteredPassword.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
