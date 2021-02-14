using ExtraMessenger.Data;
using ExtraMessenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using ExtraMessenger.DTOs;
using System.Security.Claims;

namespace ExtraMessenger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly MongoService _mongoService;

        public UserController(MongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var db = _mongoService.GetDb;

            var userCollection = db.GetCollection<User>("Users");

            
            // var users = await userCollection.FindAsync<User>(new BsonDocument());
            var filter = Builders<User>.Filter.Empty;
            var users = await userCollection.FindAsync<User>(filter);

            var usersList = await users.ToListAsync();

            var usersToReturn = usersList.Where(user => user.Id != currentUser).Select(user => 
            new UserDTO 
            { 
                ObjectId = user.Id.ToString(), 
                Username = user.Username 
            });

            return Ok(usersToReturn);
        }
    }
}
