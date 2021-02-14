using ExtraMessenger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Services.Authentication.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User> Login(string username, string password);
        Task<User> Register(string username, string password);
    }
}
