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

        [HttpGet("explore")]
        public async Task<IActionResult> ExploreUsers()
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var db = _mongoService.GetDb;

            var userCollection = db.GetCollection<User>("Users");


            // var users = await userCollection.FindAsync<User>(new BsonDocument());
            var filterContacts = Builders<User>.Filter.Eq(u => u.Id, currentUser);
            //var users = await userCollection.FindAsync<User>(filter);
            User currentDocument = (await userCollection.FindAsync<User>(filterContacts)).First<User>();
            var contactUsers = currentDocument.Contacts;

            List<User> users;
            if (contactUsers != null)
            {
                var contactsArray = currentDocument.Contacts.Select(x => x.OtherUserId);

                var filter = Builders<User>.Filter.And(!filterContacts & Builders<User>.Filter.Nin("_id", contactsArray));
                users = await userCollection.Aggregate<User>().Match(filter)
                                                                  .Limit(20)
                                                                  .ToListAsync();
                                                                //.ToString();
            }
            else
            {
                users = await userCollection.Aggregate<User>().Match(!filterContacts)
                                                                  .Limit(20)
                                                                  .ToListAsync();
                //.ToString();
            }
            return Ok(users.Select(x => new UserDTO { ObjectId = x.Id.ToString(), Username = x.Username }));
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
