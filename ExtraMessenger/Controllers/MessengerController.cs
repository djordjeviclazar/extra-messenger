using ExtraMessenger.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessengerController:ControllerBase
    {
        private readonly MongoService _context;

        public MessengerController(MongoService context)
        {
            _context = context;
        }
    }
}
