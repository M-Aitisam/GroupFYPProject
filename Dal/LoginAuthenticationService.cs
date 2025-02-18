using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Dal
{
    public class LoginAuthenticationService
    {
        private readonly ConcurrentDictionary<string, (string Email, string HashedPassword)> _users;

        public LoginAuthenticationService()
        {
            _users = new ConcurrentDictionary<string, (string Email, string HashedPassword)>();
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return false;

            if (_users.ContainsKey(username))
                return false;

            string hashedPassword = ComputeSha256Hash(password);
            _users.TryAdd(username, (email, hashedPassword));

            await Task.Delay(100);
            return true;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            string hashedPassword = ComputeSha256Hash(password);

            if (_users.TryGetValue(username, out var userData) && hashedPassword == userData.HashedPassword)
            {
                await Task.Delay(100);
                return true;
            }

            return false;
        }

        private static string ComputeSha256Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
