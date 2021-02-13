using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using ExtraMessenger.Services.Authentication.Interfaces;
using ExtraMessenger.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ExtraMessenger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IConfiguration _configuration;

        public AuthenticationController(IAuthenticationService authenticationService,
            IConfiguration configuration)
        {
            _authenticationService = authenticationService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserRegisterLoginDTO userLoginInfo)
        {
            bool result;
            string message;
            string jwt = null;
            try
            {
                result = await _authenticationService.Login(userLoginInfo.Username, userLoginInfo.Password);
                if (result)
                {
                    message = "Successfully logged in.";
                    jwt = GenerateToken(userLoginInfo);
                }
                else
                    message = "Invalid username/password combination.";
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }

            return Ok(new { Status = result, Message = message, Token = jwt });
        }

        private string GenerateToken(UserRegisterLoginDTO authenticatedUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, authenticatedUser.Username),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(createdToken);
        }
    }
}
