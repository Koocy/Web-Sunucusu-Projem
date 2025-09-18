using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI_Projem.Models;

namespace WebAPI_Projem.Controllers
{
    [Route("/")]
    public class UsersController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(DataRepo.GetUsers());
        }

        // GET /5
        [HttpGet("{ID}")]
        public IActionResult Get(int id)
        {
            return Ok(DataRepo.GetUserByID(id));
        }

        // POST 
        [HttpPost("/add")]
        public void Post([FromBody] User user)
        {
            DataRepo.AddUsers(user);
        }

        [HttpGet("/a")]
        public IActionResult GetA()
        {
            return Ok(DataRepo.GetAlphabetical());
        }
    }
}
