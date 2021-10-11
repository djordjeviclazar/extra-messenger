using ExtraMessenger.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using Neo4jClient;
using Octokit;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExtraMessenger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GithubAuthorizeController : ControllerBase
    {
        private readonly MongoService _mongoService;
        private readonly IGraphClient _graphClient;
        private readonly IConfiguration _configuration;

        public GithubAuthorizeController(IConfiguration configuration,
            MongoService mongoService,
            IGraphClient graphClient)
        {
            _mongoService = mongoService;
            _graphClient = graphClient;
            _configuration = configuration;
        }

        [HttpPost("authorize")]
        public async Task<IActionResult> Аuthorize()
        {
            /*
            bool result;
            string message;
            string jwt = null;
            try
            {
                var user = await _authenticationService.Login(userLoginInfo.Username, userLoginInfo.Password);
                if (user != null)
                {
                    result = true;
                    message = "Successfully logged in.";
                    jwt = GenerateToken(user);
                }
                else
                {
                    result = false;
                    message = "Invalid username/password combination.";
                }
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }

            return Ok(new { Status = result, Message = message, Token = jwt });*/
            var header = _configuration.GetSection("GithubOAuth:AppHeader").Value;
            GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue(_configuration.GetSection("GithubOAuth:AppHeader").Value));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value);
            Random random = new Random();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, random.Next(10000, 3000000) + ""),
                    new Claim(ClaimTypes.NameIdentifier, random.Next(10000, 3000000) + ""),
                    new Claim(ClaimTypes.UserData, random.Next(10000, 3000000) + "")
                }),
                Expires = DateTime.UtcNow.AddDays(14),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);
            

            string csrf = tokenHandler.WriteToken(createdToken);

            var aaa = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var db = _mongoService.GetDb;

            var userCollection = db.GetCollection<Models.User>("Users");

            var filter = Builders<Models.User>.Filter.Eq("Id", currentUser);

            var update = Builders<Models.User>.Update.Set("CSRF", csrf);
            await userCollection.UpdateOneAsync(filter, update);

            var request = new OauthLoginRequest(_configuration.GetSection("GithubOAuth:ClientId").Value)
            {
                Scopes = { "repo", "notifications", "user" },
                State = csrf
            };
            var oauthLoginUrl = gitHubClient.Oauth.GetGitHubLoginUrl(request);

            return Ok(oauthLoginUrl);
        }

        [HttpPost("addoauthtoken")]
        public async Task<IActionResult> AddOAuthToken(string code, string state)
        {
            GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue(_configuration.GetSection("GithubOAuth:AppHeader").Value));
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var db = _mongoService.GetDb;

            var userCollection = db.GetCollection<Models.User>("Users");

            var filter = Builders<Models.User>.Filter.Eq("Id", currentUser);

            var user = (await userCollection.FindAsync<Models.User>(filter)).FirstOrDefault();

            if (!user.CSRF.Equals(state)) { throw new InvalidOperationException("SECURITY FAIL!"); }
            //Session["CSRF:State"]

            var updateFilter = Builders<Models.User>.Filter.Eq("Id", currentUser);
            
            var token = await gitHubClient.Oauth.CreateAccessToken(
                new OauthTokenRequest(_configuration.GetSection("GithubOAuth:ClientId").Value,
                    _configuration.GetSection("GithubOAuth:ClientSecret").Value, 
                    code));

            var update = Builders<Models.User>.Update.Set("OAuthToken", token);
            await userCollection.UpdateOneAsync(filter, update);
            return Ok();
        }
    }
}
