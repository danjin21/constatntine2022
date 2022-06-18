using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedDB
{
    public class SharedDbContext : DbContext
    {
        public DbSet<TokenDb> Tokens { get; set; }
        public DbSet<ServerDb> Servers { get; set; }


        // 게임서버의 기본 생성자
        public SharedDbContext()
        {

        }


        // ASP.NET : 기본 생성자
        public SharedDbContext(DbContextOptions<SharedDbContext> options) : base(options)
        {

        }


        // 게임서버를 위한 부분
        public static string ConnectionString { get; set; } = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=SharedDB_Constantine;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {

            // 중복하여 실행하지 않도록.
            // ASP.NET으로 실행하면 IsConfigured가 true가 될 것임.
            if (options.IsConfigured == false)
            {

                options
                    //.UseLoggerFactory(_logger)  //☆★_logger
                    .UseSqlServer(ConnectionString);
                // 서버 연결되기 전에는 null 되므로 _connectionString 에서 불러오고, 서버 연결된 후에는 config 통해 갖고옴. 연결 문자열을 넣는거임.
            }



        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // 토큰의 AccountDbId 에 인덱스 아이디를 심음
            builder.Entity<TokenDb>()
                .HasIndex(t=>t.AccountDbId)
                .IsUnique();


            // 토큰의 Name 에 인덱스 아이디를 심음
            builder.Entity<ServerDb>()
                .HasIndex(s=>s.Name)
                .IsUnique();
        }
    }
}
