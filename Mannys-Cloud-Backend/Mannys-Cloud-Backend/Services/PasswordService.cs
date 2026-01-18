using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Models;
using System.Security.Cryptography;
using System.Text;

namespace Mannys_Cloud_Backend.Services
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return hashedPassword == this.HashPassword(password);
        }
    }
}
