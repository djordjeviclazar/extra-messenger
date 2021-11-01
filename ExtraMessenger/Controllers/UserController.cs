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
using Neo4jClient;

namespace ExtraMessenger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly MongoService _mongoService;
        private readonly IGraphClient _neoContext;

        public UserController(MongoService mongoService, IGraphClient graphClient)
        {
            _mongoService = mongoService;
            _neoContext = graphClient;
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

            int sampleSize = 5;
            List<User> users;
            if (contactUsers != null)
            {
                var contactsArray = currentDocument.Contacts.Select(x => x.OtherUserId);

                var filter = Builders<User>.Filter.And(!filterContacts & Builders<User>.Filter.Nin("_id", contactsArray));
                users = await userCollection.Aggregate<User>().Match(filter)
                                                                //.Limit(20)
                                                                  .AppendStage<User>($@"{{ $sample: {{ size: {sampleSize} }} }}")
                                                                  .ToListAsync();
                                                                //.ToString();
            }
            else
            {
                users = await userCollection.Aggregate<User>().Match(!filterContacts)
                                                                  .Limit(20)
                                                                  .AppendStage<User>($@"{{ $sample: {{ size: {sampleSize} }} }}")
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

        [HttpGet("getprofile")]
        public async Task<IActionResult> GetProfile()
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // basic info
            var mongoDb = _mongoService.GetDb;
            var mongoSettings = _mongoService.GetDBSettings;
            var userCollection = mongoDb.GetCollection<Models.User>(mongoSettings.UserCollectionName);
            var filterUser = Builders<Models.User>.Filter.Eq(u => u.Id, currentUser);
            var user = (await userCollection.FindAsync<Models.User>(filterUser)).FirstOrDefault();

            var ratingQuery = _neoContext.Cypher
                .OptionalMatch("(u:User {Id:'" + currentUser.ToString() + "'})-[rel:CREATED]->(t:Ticket)<-[:UPVOTED]-(u2:User)")
                .With("count(u2) AS ups")
                .OptionalMatch("(u)-[rel2:CREATED]->(t:Ticket)<-[:DOWNVOTED]-(u3:User)")
                .With("ups - count(u3) AS res")
                .Return<int>("res");
            var ratingQueryText = ratingQuery.Query.DebugQueryText;
            var rating = await ratingQuery.ResultsAsync;

            var topics = await _neoContext.Cypher
                .Match("(u:User {Id:'" + currentUser.ToString() + "'})-[rel:INTERESTED]->(t:Topic)")
                .With("t.Name AS Name")
                .Return((Name) => new
                {
                    Name = Name.As<string>()
                })
                .ResultsAsync;
            var otherTopicsQuery = _neoContext.Cypher
                .Match("(u:User {Id:'" + currentUser.ToString() + "'})")
                .Match("(t:Topic)")
                .Where("NOT (u)-[:INTERESTED]->(t)")
                .With("t.Name AS Name")
                .Return((Name) => new
                {
                    Name = Name.As<string>()
                });
            var otherTopicsQueryText = otherTopicsQuery.Query.DebugQueryText;
            var otherTopics = await otherTopicsQuery
                .ResultsAsync;

            var returnDTO = new UserProfileDTO
            {
                Username = user.Username,
                LikedTopics = topics.Select(x => x.Name).ToList(),
                OtherTopics = otherTopics.Select(x => x.Name).ToList(),
                Rating = rating.ToList()[0]
            };

            return Ok(returnDTO);
        }

        [HttpGet("gettopTickets")]
        public async Task<IActionResult> GetTopTickets()
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var Tickets = await _neoContext.Cypher
                .Match("(u:User {Id:'" + currentUser.ToString() + "'})-[rel:CREATED]->(t:Ticket)")
                .With("t.Id AS id, t.Title AS title, (size(()-[:UPVOTED]->(t))- size(()-[:DOWNVOTED]->(t))) AS rating")
                .Return((id, title, rating) => new TicketRes
                {
                    Id = id.As<string>(),
                    Title = title.As<string>(),
                    Rating = rating.As<int>()
                })
                .OrderBy("rating DESC")
                .Limit(3)
                .ResultsAsync;

            return Ok(Tickets.ToList());
        }

        [HttpGet("  addinterest/{name}")]
        public async Task<IActionResult> AddInterest(string name)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await _neoContext.Cypher
                .Match("(u:User {Id:'" + currentUser.ToString() + "'})")
                .Match("(t:Topic {Name:'" + name + "'})")
                .Create("(u)-[i:INTEREST]->(t)")
                .ExecuteWithoutResultsAsync();

            return Ok(new { Success = true });
        }

        [HttpGet("removeinterest/{name}")]
        public async Task<IActionResult> RemoveInterest(string name)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await _neoContext.Cypher
                .Match("(u:User {Id:'" + currentUser.ToString() + "'})-[i:INTEREST]->(t:Topic {Name:'" + name + "'})")
                .Delete("i")
                .ExecuteWithoutResultsAsync();

            return Ok(new { Success = true });
        }
    }
}
