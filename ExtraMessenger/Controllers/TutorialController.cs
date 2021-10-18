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
    public class TutorialController : ControllerBase
    {
        private readonly IGraphClient _neoContext;
        private readonly MongoService _mongoService;

        public TutorialController(IGraphClient graphClient,
            MongoService mongoService)
        {
            _neoContext = graphClient;
            _mongoService = mongoService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(TutorialDTO tutorialDTO)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var mongoDb = _mongoService.GetDb;
            var mongoSettings = _mongoService.GetDBSettings;
            var userCollection = mongoDb.GetCollection<Models.User>(mongoSettings.UserCollectionName);
            var filterUser = Builders<Models.User>.Filter.Eq(u => u.Id, currentUser);
            var user = (await userCollection.FindAsync<Models.User>(filterUser)).FirstOrDefault();

            var tutorial = new Tutorial
            {
                Id = ObjectId.GenerateNewId(),
                Title = tutorialDTO.Title,
                Parts = tutorialDTO.Parts,
                Introduction = tutorialDTO.Introduction,
                Topics = tutorialDTO.Topics,
                Difficulty = tutorialDTO.Difficulty,
                Owner = user.Username
            };

            var tutorialsCollection = mongoDb.GetCollection<Models.Tutorial>(mongoSettings.TutorialsCollectionName);
            await tutorialsCollection.InsertOneAsync(tutorial);

            var createQuery = _neoContext.Cypher
                .Match("(u:User {Id:'" + currentUser.ToString() + "'})")
                .Match("(d:Difficulty {Name:'" + tutorial.Difficulty + "'})")
                .Unwind(tutorial.Topics, "topicsArray")
                .Match("(topic:Topic {Name: coalesce(topicsArray, 'N/A')})")
                .Create("(t:Tutorial {Id: '"+ tutorial.Id.ToString() +"' ,Title:'" + tutorial.Title + "', Time:datetime()})")
                .Merge("(u)-[r1:CREATED]->(t)")
                .Merge("(topic)<-[r2:ON_TOPIC]-(t)")
                .Merge("(d)<-[r3:HAS_DIFFICULTY]-(t)")
                .With("t")
                .Unwind(tutorial.Parts, "part")
                .Match("(r:Repo {Id:part.RepoId})")
                .Merge("(p:Part {Title:part.Title, RepoId:part.RepoId, RepoUrl:part.RepoUrl})-[r4:HAS_EXAMPLE]->(r)")
                .Merge("(t)-[r5:HAS_PART]->(p)");
            var createQueryText = createQuery.Query.DebugQueryText;
            await createQuery.ExecuteWithoutResultsAsync();

            return Ok(new { Success = true, Id = tutorial.Id.ToString() });
        }

        [HttpGet("gettutorial/{tutorialid}")]
        public async Task<IActionResult> GetTutorial(string tutorialId)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var mongoDb = _mongoService.GetDb;
            var mongoSettings = _mongoService.GetDBSettings;
            var tutorialCollection = mongoDb.GetCollection<Models.Tutorial>(mongoSettings.TutorialsCollectionName);
            var filter = Builders<Models.Tutorial>.Filter.Eq(t => t.Id, ObjectId.Parse(tutorialId));
            var tutorial = (await tutorialCollection.FindAsync<Models.Tutorial>(filter)).FirstOrDefault();

            return Ok(tutorial);
        }

        [HttpPut("upvote/{tutorialid}")]
        public async Task<IActionResult> Upvote(string tutorialId)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var mongoDb = _mongoService.GetDb;
            var mongoSettings = _mongoService.GetDBSettings;
            var tutorialCollection = mongoDb.GetCollection<Models.Tutorial>(mongoSettings.TutorialsCollectionName);
            var filter = Builders<Models.Tutorial>.Filter.Eq(t => t.Id, ObjectId.Parse(tutorialId));
            var inc = Builders<Models.Tutorial>.Update.Inc(x => x.Upvotes, 1);
            await tutorialCollection.UpdateOneAsync(filter, inc);

            var query = _neoContext.Cypher
                .Match("(t:Tutorial {Id:'" + tutorialId + "'})")
                .Match("(t:User {Id:'" + currentUser.ToString() + "'})")
                .Merge("(t)-[r:UPVOTED {Time: datetime()}]->(t)");
            var queryText = query.Query.DebugQueryText;
            await query.ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpPut("downvote/{tutorialid}")]
        public async Task<IActionResult> Downvote(string tutorialId)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var mongoDb = _mongoService.GetDb;
            var mongoSettings = _mongoService.GetDBSettings;
            var tutorialCollection = mongoDb.GetCollection<Models.Tutorial>(mongoSettings.TutorialsCollectionName);
            var filter = Builders<Models.Tutorial>.Filter.Eq(t => t.Id, ObjectId.Parse(tutorialId));
            var inc = Builders<Models.Tutorial>.Update.Inc(x => x.Downvotes, 1);
            await tutorialCollection.UpdateOneAsync(filter, inc);

            var query = _neoContext.Cypher
                .Match("(t:Tutorial {Id:'" + tutorialId + "'})")
                .Match("(t:User {Id:'" + currentUser.ToString() + "'})")
                .Merge("(t)-[r:DOWNVOTED {Time: datetime()}]->(t)");
            var queryText = query.Query.DebugQueryText;
            await query.ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpPut("isvoted/{tutorialid}")]
        public async Task<IActionResult> IsVoted(string tutorialId)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var query = _neoContext.Cypher
                .Match("(t:Tutorial {Id:'" + tutorialId + "'})")
                .Match("(t:User {Id:'" + currentUser.ToString() + "'})")
                .Return<int>("COUNT(*) > 0");
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
    }
}
