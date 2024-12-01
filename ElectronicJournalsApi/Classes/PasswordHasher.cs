using System.Security.Cryptography;
using System.Text;

namespace ElectronicJournalApi.Classes
{
    public class PasswordHasher
    {
        public static string GenerateSaltedPassword(string password, string login)
        {
            var saltedPassword = new StringBuilder();
            int maxLength = Math.Max(password.Length, login.Length);

            for (int i = 0; i < maxLength; i++)
            {
                if (i < login.Length)
                {
                    saltedPassword.Append(login[i]);
                }
                if (i < password.Length)
                {
                    saltedPassword.Append(password[i]);
                }
            }

            return saltedPassword.ToString();
        }

        public static byte[] HashPassword(string password, string login)
        {
            // Генерируем соль
            string saltedPassword = GenerateSaltedPassword(password, login);

            // Хешируем с помощью SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            }
        }

        public static bool VerifyPassword(string enteredPassword, string login, byte[] storedHash)
        {
            // Хешируем введенный пароль с использованием того же логина
            byte[] hashedEnteredPassword = HashPassword(enteredPassword, login);

            // Сравниваем хеши
            return hashedEnteredPassword.SequenceEqual(storedHash);
        }
    }
}
