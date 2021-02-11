using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Services.Authentication.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> Login(string username, string password);
        Task<bool> Register(string username, string password);
    }
}
