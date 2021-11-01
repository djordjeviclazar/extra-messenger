using ExtraMessenger.Data;
using ExtraMessenger.DTOs;
using ExtraMessenger.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExtraMessenger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly IGraphClient _neoContext;
        private readonly MongoService _mongoService;

        public TicketController(IGraphClient graphClient,
            MongoService mongoService)
        {
            _neoContext = graphClient;
            _mongoService = mongoService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(TicketDTO TicketDTO)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var mongoDb = _mongoService.GetDb;
            var mongoSettings = _mongoService.GetDBSettings;
            var userCollection = mongoDb.GetCollection<Models.User>(mongoSettings.UserCollectionName);
            var filterUser = Builders<Models.User>.Filter.Eq(u => u.Id, currentUser);
            var user = (await userCollection.FindAsync<Models.User>(filterUser)).FirstOrDefault();

            var Ticket = new Ticket
            {
                Id = ObjectId.GenerateNewId(),
                Title = TicketDTO.Title,
                Parts = TicketDTO.Parts,
                Introduction = TicketDTO.Introduction,
                Topics = TicketDTO.Topics,
                Difficulty = TicketDTO.Difficulty,
                Owner = user.Username
            };

            var TicketsCollection = mongoDb.GetCollection<Models.Ticket>(mongoSettings.TicketsCollectionName);
            await TicketsCollection.InsertOneAsync(Ticket);

            var createQuery = _neoContext.Cypher
                .Match("(u:User {Id:'" + currentUser.ToString() + "'})")
                .Match("(d:Difficulty {Name:'" + Ticket.Difficulty + "'})")
                .Unwind(Ticket.Topics, "topicsArray")
                .Match("(topic:Topic {Name: coalesce(topicsArray, 'N/A')})")
                .Create("(t:Ticket {Id: '"+ Ticket.Id.ToString() +"' ,Title:'" + Ticket.Title + "', Time:datetime()})")
                .Merge("(u)-[r1:CREATED]->(t)")
                .Merge("(topic)<-[r2:ON_TOPIC]-(t)")
                .Merge("(d)<-[r3:HAS_DIFFICULTY]-(t)")
                .With("t")
                .Unwind(Ticket.Parts, "part")
                .Match("(r:Repo {Id:part.RepoId})")
                .Merge("(p:Part {Title:part.Title, RepoId:part.RepoId, RepoUrl:part.RepoUrl})-[r4:HAS_EXAMPLE]->(r)")
                .Merge("(t)-[r5:HAS_PART]->(p)");
            var createQueryText = createQuery.Query.DebugQueryText;
            await createQuery.ExecuteWithoutResultsAsync();

            return Ok(new { Success = true, Id = Ticket.Id.ToString() });
        }

        [HttpGet("getTicket/{Ticketid}")]
        public async Task<IActionResult> GetTicket(string TicketId)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var mongoDb = _mongoService.GetDb;
            var mongoSettings = _mongoService.GetDBSettings;
            var TicketCollection = mongoDb.GetCollection<Models.Ticket>(mongoSettings.TicketsCollectionName);
            var filter = Builders<Models.Ticket>.Filter.Eq(t => t.Id, ObjectId.Parse(TicketId));
            var Ticket = (await TicketCollection.FindAsync<Models.Ticket>(filter)).FirstOrDefault();

            return Ok(Ticket);
        }

        [HttpPut("upvote/{TicketId}")]
        public async Task<IActionResult> Upvote(string TicketId)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var mongoDb = _mongoService.GetDb;
            var mongoSettings = _mongoService.GetDBSettings;
            var TicketCollection = mongoDb.GetCollection<Models.Ticket>(mongoSettings.TicketsCollectionName);
            var filter = Builders<Models.Ticket>.Filter.Eq(t => t.Id, ObjectId.Parse(TicketId));
            var inc = Builders<Models.Ticket>.Update.Inc(x => x.Upvotes, 1);
            await TicketCollection.UpdateOneAsync(filter, inc);

            var query = _neoContext.Cypher
                .Match("(t:Ticket {Id:'" + TicketId + "'})")
                .Match("(u:User {Id:'" + currentUser.ToString() + "'})")
                .Merge("(u)-[r:UPVOTED {Time: datetime()}]->(t)");
            var queryText = query.Query.DebugQueryText;
            await query.ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpPut("downvote/{TicketId}")]
        public async Task<IActionResult> Downvote(string TicketId)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var mongoDb = _mongoService.GetDb;
            var mongoSettings = _mongoService.GetDBSettings;
            var TicketCollection = mongoDb.GetCollection<Models.Ticket>(mongoSettings.TicketsCollectionName);
            var filter = Builders<Models.Ticket>.Filter.Eq(t => t.Id, ObjectId.Parse(TicketId));
            var inc = Builders<Models.Ticket>.Update.Inc(x => x.Downvotes, 1);
            await TicketCollection.UpdateOneAsync(filter, inc);

            var query = _neoContext.Cypher
                .Match("(t:Ticket {Id:'" + TicketId + "'})")
                .Match("u:User {Id:'" + currentUser.ToString() + "'})")
                .Merge("(u)-[r:DOWNVOTED {Time: datetime()}]->(t)");
            var queryText = query.Query.DebugQueryText;
            await query.ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpGet("isvoted/{Ticketid}")]
        public async Task<IActionResult> IsVoted(string TicketId)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var query = _neoContext.Cypher
                .Match("(t:Ticket {Id:'" + TicketId + "'})")
                .OptionalMatch("(u:User {Id:'" + currentUser.ToString() + "'})-[up:UPVOTED]->(t)")
                .OptionalMatch("(u)-[down:DOWNVOTED]->(t)")
                .With("(count(up) > 0) as ups, (count(down) > 0) as downs")
                .Return((ups, downs) => new
                {
                    Upvoted = ups.As<bool>(),
                    Downvoted = downs.As<bool>()
                });
            var queryText = query.Query.DebugQueryText;

            return Ok(await query.ResultsAsync);
        }

        [HttpGet("gettopics")]
        public async Task<IActionResult> GetTopics()
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var query = _neoContext.Cypher
                .Match("(n:Topic)")
                .With("n.Name AS name")
                .Return((name) => new { Name = name.As<string>() });
            return Ok(await query.ResultsAsync);
        }

        [HttpGet("getdifficulties")]
        public async Task<IActionResult> GetDifficulties()
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var query = _neoContext.Cypher
                .Match("(n:Difficulty)")
                .With("n.Name AS name")
                .Return((name) => new { Name = name.As<string>() });
            return Ok(await query.ResultsAsync);
        }

        [HttpGet("getlanguages")]
        public async Task<IActionResult> GetLanguages()
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var query = _neoContext.Cypher
                .Match("(n:Language)")
                .With("n.Name AS name")
                .Return((name) => new { Name = name.As<string>() });
            return Ok(await query.ResultsAsync);
        }

        [HttpGet("getrecommended")]
        public async Task<IActionResult> GetRecommended()
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            DateTime dateTime = DateTime.UtcNow;
            var beforeLimit = dateTime.AddDays(-14);
            var beforeLimitString = beforeLimit.ToString("yyyy-MM-dd") + "T" + beforeLimit.ToString("HH:mm:ss.ff") + "Z";
            var query = _neoContext.Cypher
                .Match($"(u:User {{Id: '{currentUser}'}})-[:INTEREST]->(rec:Topic)<-[:ON_TOPIC]-(t:Ticket)")
                .Where($"(t.Time > datetime('{beforeLimitString}')) AND (NOT (u)-[:CREATED]->(t))")
                .Match($"(c)-[:CREATED]->(t)")
                .With($"c.Username AS username, c.Id as userId, t.Id as TicketId, t.Title as title")
                .Return((username, userId, TicketId, title) => new 
                {
                    Username = username.As<string>(),
                    UserId = userId.As<string>(),
                    TicketId = TicketId.As<string>(),
                    Title = title.As<string>()
                })
                .Limit(10);
            var queryText = query.Query.DebugQueryText;


            return Ok(await query.ResultsAsync);
        }

        [HttpGet("gethot")]
        public async Task<IActionResult> GetHot()
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            DateTime dateTime = DateTime.UtcNow;
            var beforeLimit = dateTime.AddDays(-5);
            var beforeLimitString = beforeLimit.ToString("yyyy-MM-dd") + "T" + beforeLimit.ToString("HH:mm:ss.ff") + "Z";
            var query = _neoContext.Cypher
                .Match($"(u:User)-[up:UPVOTED]->(t:Ticket)")
                .Where($"(up.Time > datetime('{beforeLimitString}')) AND (NOT (u)-[:CREATED]->(t))")
                .Match($"(c)-[:CREATED]->(t)")
                .With($"count(up) as ups, c.Username AS username, c.Id as userId, t")
                .OptionalMatch($"(u2:User)-[down:DOWNVOTED]->(t)")
                .With($"ups, (ups * 10 - coalesce(count(down),0) * 2) as rating, username, userId, t.Id as TicketId, t.Title as title")
                .Where("rating >= 0")
                .Return((username, rating, userId, TicketId, title) => new
                {
                    Username = username.As<string>(),
                    Rating = rating.As<int>(),
                    UserId = userId.As<string>(),
                    TicketId = TicketId.As<string>(),
                    Title = title.As<string>()
                })
                .OrderByDescending("rating", "ups")
                .Limit(5);
            var queryText = query.Query.DebugQueryText;

            return Ok(await query.ResultsAsync);
        }

        [HttpGet("basicstats")]
        public async Task<IActionResult> GetBasicStats()
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var query = _neoContext.Cypher
                .Match("(l: Language)")
                .OptionalMatch($"(l)<-[:WRITTEN_IN]-(r2:Repo)<-[:OWNS]-(u:User {{Id: '{currentUser}'}})")
                .OptionalMatch($"(l)<-[:WRITTEN_IN]-(r:Repo)<-[:HAS_EXAMPLE]-(p:Part)<-[:HAS_PART]-(t:Ticket)<-[:UPVOTED]-(u )")
                .With($"count(r2) AS work, count(p) AS interest, l.Name AS language")
                .Return((work, interest, language) => new
                {
                    Work = work.As<int>(),
                    Interest = interest.As<int>(),
                    Language = language.As<string>()
                });
            var queryText = query.Query.DebugQueryText;
            var result = (await query.ResultsAsync).ToList();

            var labels = result.Select(x => x.Language);
            var Tickets = result.Select(x => x.Interest);
            var repos = result.Select(x => x.Work);

            return Ok(new { labels = labels, TicketArray = Tickets, repoArray = repos });
        }
    }
}
