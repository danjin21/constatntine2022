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
    public class RegisterController : ControllerBase
    {

		ApplicationDbContext _context;

		public RegisterController(ApplicationDbContext context)
		{
			_context = context;
		}

		// Create
		[HttpPost]
		public User AddUser([FromBody] User user)
		{
            Console.WriteLine("User Data : " + user);

			var hash = new PasswordHasher<string>();
			var hashpassword = hash.HashPassword(null, user.Password);


			//var result = hash.VerifyHashedPassword(null, hashpassword, user.Password);

			User idCheck = _context.Users
				.Where(t => t.UserId == user.UserId || t.Nickname == user.Nickname)
				.FirstOrDefault();

			if(idCheck != null )
				return null;

			user.Password = hashpassword;

			_context.Users.Add(user);
			_context.SaveChanges();

			return user;
		}

	}
}
