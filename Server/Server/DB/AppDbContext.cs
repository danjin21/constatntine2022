using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DB
{
    public class AppDbContext : DbContext
    {

        public DbSet<AccountDb> Accounts { get; set; }
        public DbSet<PlayerDb> Players { get; set; }
        public DbSet<ItemDb> Items { get; set; }
        public DbSet<KeySettingDb> Keys { get; set; }
        public DbSet<SkillDb> Skills { get; set; }
        public DbSet<QuestDb> Quests { get; set; }
        public DbSet<QuestMobCountDb> QuestMobCounts { get; set; }

        // 새로운 콘솔 로깅을 ☆★_logger 에 넣는다.
        static readonly ILoggerFactory _logger = LoggerFactory.Create(
            builder => { builder.AddConsole(); });

        // 서버 => 속성 => 연결문자열을 붙여넣기 catalog = 원하는 DB명으로 수정
        string _connectionString = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=ConstantineDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options
                .UseLoggerFactory(_logger)  //☆★_logger DB 정보 보여주는 곳
                .UseSqlServer(ConfigManager.Config == null ? _connectionString : ConfigManager.Config.connectionString);
                // 서버 연결되기 전에는 null 되므로 _connectionString 에서 불러오고, 서버 연결된 후에는 config 통해 갖고옴. 연결 문자열을 넣는거임.



        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // 어느새 추가됨.
            builder.Entity<AccountDb>()
                .HasIndex(a => a.AccountName)
                .IsUnique();

            builder.Entity<PlayerDb>()
                .HasIndex(p => p.PlayerName)
                .IsUnique(); // 유니크 인덱스를 붙여넣는다.
        }

    }
}
