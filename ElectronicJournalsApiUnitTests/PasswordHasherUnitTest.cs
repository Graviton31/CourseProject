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
            // Arrange: �������������� ������� ������ ��� �����
            string password = "password123";
            string login = "user@example.com";
            string expected = "upsaesrs@weoxradm1p2l3e.com";

            // Act: �������� �����, ������� �� ���������
            string result = PasswordHasher.GenerateSaltedPassword(password, login);

            // Assert: ���������, ��� ��������� ������������� ���������� ��������
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HashPasswordDiff()
        {
            // Arrange: �������������� ������ ������� ������ ��� �����
            string password1 = "password123";
            string login1 = "user@example.com";
            string password2 = "password456";
            string login2 = "user@example.com";

            // Act: �������� ����� ����������� ��� ����� �������
            byte[] hash1 = PasswordHasher.HashPassword(password1, login1);
            byte[] hash2 = PasswordHasher.HashPassword(password2, login2);

            // Assert: ���������, ��� ���� ��������
            Assert.False(hash1.SequenceEqual(hash2));
        }

        [Fact]
        public void VerifyPasswordValid()
        {
            // Arrange: �������������� ������� ������ ��� �����
            string password = "password123";
            string login = "user@example.com";
            byte[] storedHash = PasswordHasher.HashPassword(password, login);

            // Act: ��������� ��������� ������
            bool result = PasswordHasher.VerifyPassword(password, login, storedHash);

            // Assert: ���������, ��� ����� ���������� true ��� ����������� ������
            Assert.True(result);
        }

        [Fact]
        public void VerifyPasswordInvalid()
        {
            // Arrange: �������������� ������� ������ ��� �����
            string password = "password123";
            string login = "user@example.com";
            byte[] storedHash = PasswordHasher.HashPassword(password, login);
            string wrongPassword = "wrongpassword";

            // Act: ��������� ��������� ������������ ������
            bool result = PasswordHasher.VerifyPassword(wrongPassword, login, storedHash);

            // Assert: ���������, ��� ����� ���������� false ��� ������������� ������
            Assert.False(result);
        }
    }
}
