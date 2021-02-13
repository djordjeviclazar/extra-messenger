using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtraMessenger.Data;
using ExtraMessenger.Models;
using ExtraMessenger.Services.Authentication.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExtraMessenger.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly MongoService _mongoService;

        public AuthenticationService(MongoService mongoService)
        {
            _mongoService = mongoService;
        }

        public async Task<User> Login(string username, string password)
        {
            var db = _mongoService.GetDb;
            var userCollection = db.GetCollection<User>("Users");

            var filter = Builders<User>.Filter.Eq("Username", username);

            var user = (await userCollection.FindAsync<User>(filter)).FirstOrDefault();

            if (user == null)
                return null;

            if (!ValidatePassword(user, password))
                return null;

            return user;
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

        public async Task<User> Register(string username, string password)
        {
            var db = _mongoService.GetDb;
            var userCollection = db.GetCollection<User>("Users");

            var filter = Builders<User>.Filter.Eq("Username", username);

            var user = (await userCollection.FindAsync<User>(filter)).FirstOrDefault();

            if (user != null)
                return null;

            GeneratePassword(password, out byte[] passwordHash, out byte[] passwordSalt);

            User registeredUser = new User
            {
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Username = username,
                Id = ObjectId.GenerateNewId()
            };

            await userCollection.InsertOneAsync(registeredUser);

            return registeredUser;
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
