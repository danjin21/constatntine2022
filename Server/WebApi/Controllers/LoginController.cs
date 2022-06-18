using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApiData.Models;
using WebApi.Data;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

		ApplicationDbContext _context;

		public LoginController(ApplicationDbContext context)
		{
			_context = context;
		}

		// Create
		[HttpPost]
		public User CheckUser([FromBody] User user)
		{
            Console.WriteLine("User Data : " + user);




			var hash = new PasswordHasher<string>();
			var hashpassword = hash.HashPassword(null, user.Password);



			User idCheck = _context.Users
				.Where(t => t.UserId == user.UserId )
				.FirstOrDefault();

			if(idCheck == null )
				return null;

			var rst = hash.VerifyHashedPassword(null, hashpassword, idCheck.Password);


			return idCheck;
		}

		// Read
		[HttpGet("{id}")]
		public User GetUser(int id)
        {
			User user = _context.Users
				.Where(user => user.Id == id)
				.FirstOrDefault();

			if (user == null)
				return null;

			return user;
        }




	}
}
