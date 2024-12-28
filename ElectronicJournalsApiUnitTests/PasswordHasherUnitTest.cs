using ElectronicJournalApi.Classes;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace ElectronicJournalApi.Tests
{
    public class PasswordHasherTests
    {
        [Fact]
        public void GenerateSaltedPasswordCombines()
        {
            // Arrange: Подготавливаем входные данные для теста
            string password = "password123";
            string login = "user@example.com";
            string expected = "upsaesrs@weoxradm1p2l3e.com";

            // Act: Вызываем метод, который мы тестируем
            string result = PasswordHasher.GenerateSaltedPassword(password, login);

            // Assert: Проверяем, что результат соответствует ожидаемому значению
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HashPasswordDiff()
        {
            // Arrange: Подготавливаем разные входные данные для теста
            string password1 = "password123";
            string login1 = "user@example.com";
            string password2 = "password456";
            string login2 = "user@example.com";

            // Act: Вызываем метод хеширования для обоих паролей
            byte[] hash1 = PasswordHasher.HashPassword(password1, login1);
            byte[] hash2 = PasswordHasher.HashPassword(password2, login2);

            // Assert: Проверяем, что хеши различны
            Assert.False(hash1.SequenceEqual(hash2));
        }

        [Fact]
        public void VerifyPasswordValid()
        {
            // Arrange: Подготавливаем входные данные для теста
            string password = "password123";
            string login = "user@example.com";
            byte[] storedHash = PasswordHasher.HashPassword(password, login);

            // Act: Проверяем введенный пароль
            bool result = PasswordHasher.VerifyPassword(password, login, storedHash);

            // Assert: Проверяем, что метод возвращает true для правильного пароля
            Assert.True(result);
        }

        [Fact]
        public void VerifyPasswordInvalid()
        {
            // Arrange: Подготавливаем входные данные для теста
            string password = "password123";
            string login = "user@example.com";
            byte[] storedHash = PasswordHasher.HashPassword(password, login);
            string wrongPassword = "wrongpassword";

            // Act: Проверяем введенный неправильный пароль
            bool result = PasswordHasher.VerifyPassword(wrongPassword, login, storedHash);

            // Assert: Проверяем, что метод возвращает false для неправильного пароля
            Assert.False(result);
        }
    }
}
