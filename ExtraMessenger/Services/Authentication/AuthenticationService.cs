using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtraMessenger.Models;
using ExtraMessenger.Services.Authentication.Interfaces;

namespace ExtraMessenger.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {

        public AuthenticationService()
        {
        }

        public async Task<bool> Login(string username, string password)
        {
            //using (var session = _cassandraDbConnectionProvider.Connect())
            //{
            //    string cql = "SELECT * FROM users WHERE username = ?";
            //    var user = await _cassandraQueryProvider.QuerySingleOrDefault<User>(session, cql, username);

            //    if (user == null)
            //        return false;

            //    if (!ValidatePassword(user, password))
            //        return false;
            //}

            return true;
        }

        private bool ValidatePassword(User user, string password)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt))
            {
                var enteredPasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                if (user.PasswordHash.Length != enteredPasswordHash.Length)
                    return false;

                for (int i = 0; i < enteredPasswordHash.Length; i++)
                {
                    if (enteredPasswordHash[i] != user.PasswordHash[i])
                        return false;
                }
                return true;
            }
        }

        public async Task<bool> Register(string username, string password)
        {
            GeneratePassword(password, out byte[] passwordHash, out byte[] passwordSalt);

            //using (var session = _cassandraDbConnectionProvider.Connect())
            //{
            //    string cql = "INSERT INTO users (username, passwordhash, passwordsalt) VALUES (?, ?, ?)";
            //    var newUser = await _cassandraQueryProvider.ExecuteAsync(session, cql, username, passwordHash, passwordSalt);

            //    if (newUser == null)
            //        return false;
            //}

            return true;
        }

        private void GeneratePassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
