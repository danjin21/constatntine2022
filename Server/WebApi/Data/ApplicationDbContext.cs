using Microsoft.EntityFrameworkCore;
using WebApiData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<GameResult> GameResults { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<BoardContent> BoardContents { get; set; }
		public DbSet <SiteData> SiteData { get; set; }
		public DbSet <Comment> Comments { get; set; }


		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{

		}
	}
}
