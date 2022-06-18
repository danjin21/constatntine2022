using AccountServer.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        AppDbContext _context;
        SharedDbContext _shared;

        public AccountController(AppDbContext context, SharedDbContext shared)
        {
            _context = context;
            _shared = shared;
        }

        [HttpPost]
        [Route("create")]
        public CreateAccountPacketRes CreateAccount([FromBody] CreateAccountPacketReq req)
        {
            CreateAccountPacketRes res = new CreateAccountPacketRes();

            AccountDb account = _context.Accounts
                            .AsNoTracking() // 읽기전용
                            .Where(a => a.AccountName == req.AccountName)
                            .FirstOrDefault();


            if (account == null)
            {
                // 계정 생성
                _context.Accounts.Add(new AccountDb()
                {
                    AccountName = req.AccountName,
                    Password = req.Password
                });

                bool success = _context.SaveChangesEx();
                res.CreateOk = success;
                res.Info = "ID 생성 성공! 로그인 해주세요.";
            }
            else
            {
                res.CreateOk = false;
                res.Info = "이미 존재하는 ID 입니다.";
            }


            return res;
        }

        [HttpPost]
        [Route("login")]
        public LoginAccountPacketRes LoginAccount([FromBody] LoginAccountPacketReq req)
        {
            LoginAccountPacketRes res = new LoginAccountPacketRes();

            AccountDb account = _context.Accounts
                .AsNoTracking()
                .Where(a => a.AccountName == req.AccountName && a.Password == req.Password)
                .FirstOrDefault();

            if(account == null)
            {
                res.LoginOk = false;
                res.Info = "존재하지 않는 ID 이거나, 비밀번호가 틀렸습니다.";
            }
            else
            {


                // 토큰 업데이트하거나 추가하는 부분
                TokenDb tokenDb = _shared.Tokens.Where(t => t.AccountDbId == account.AccountDbId).FirstOrDefault();

                if (tokenDb != null && tokenDb.IsLogin == true)
                {
                    res.LoginOk = false;
                    res.Info = "접속중인 ID 입니다. 잠시 후 재시도 해주세요.";
                }
                else if(tokenDb == null || tokenDb.IsLogin == false)
                {


                    // 토큰 발급
                    DateTime expired = DateTime.UtcNow; // 로컬시간 / 절대시간 or Utc
                    expired.AddSeconds(600); // 600초 뒤에 만료됨


                    if (tokenDb != null)
                    {
                        // 인증받을 수 있는 번호표 , 거의 맞아떨어질일이 거의 없음
                        tokenDb.Token = new Random().Next(Int32.MinValue, Int32.MaxValue);
                        tokenDb.Expired = expired;
                        tokenDb.AccountName = account.AccountName;


                        // 게임 접속 하자마자도 isLogin 해주지만, 서버 선택할때(웹도그렇고) 부터 바로 IsLogin을 해준다.
                        //tokenDb.IsLogin = true;


                        tokenDb.Password = account.Password; // 토큰DB에 비번 저장해서 나중에도 써먹을 수 있도록
                        _shared.SaveChangesEx();


                    }
                    else // 유저가 처음 접근했을때
                    {
                        tokenDb = new TokenDb()
                        {
                            AccountDbId = account.AccountDbId,
                            Token = new Random().Next(Int32.MinValue, Int32.MaxValue),
                            Expired = expired,

                            AccountName = account.AccountName,
                            // IsLogin은 게임 안으로 접속되었을 떄 해주기.
                            //IsLogin = true,
                            Password = account.Password // 토큰DB에 비번 저장해서 나중에도 써먹을 수 있도록

                            // 클라이언트한테 AccountDbId 와 Token 을 준다.
                            // 너가 이제 게임서버 접근하려면 이 Token을 제시해줘야해.

                        };
                        _shared.Add(tokenDb);
                        _shared.SaveChangesEx();
                    }

                    res.LoginOk = true;
                    res.AccountId = account.AccountDbId;
                    res.Token = tokenDb.Token;
                    res.AccountName = account.AccountName;
                    res.ServerList = new List<ServerInfo>();

                    foreach (ServerDb serverDb in _shared.Servers)
                    {
                        res.ServerList.Add(new ServerInfo()
                        {
                            Name = serverDb.Name,
                            IpAddress = serverDb.IpAddress,
                            Port = serverDb.Port,
                            BusyScore = serverDb.BusyScore
                        });

                    }


                    // TODO 서버 목록
                    //res.ServerList = new List<ServerInfo>()
                    //{
                    //    new ServerInfo() { Name = "인트", Ip = "127.0,0.1", CrowdedLevel = 0},
                    //    new ServerInfo() { Name = "세인트", Ip = "127.0,0.1", CrowdedLevel = 3}
                    //};
                }

            }

                return res;
        }




        [HttpPost]
        [Route("logout")]
        public LogoutAccountPacketRes Logout([FromBody] LogoutAccountPacketReq req)
        {
            LogoutAccountPacketRes res = new LogoutAccountPacketRes();

            AccountDb account = _context.Accounts
                            .AsNoTracking() // 읽기전용
                            .Where(a => a.AccountName == req.AccountName)
                            .FirstOrDefault();


            if (account == null)
            {
                res.LogoutOk = false;
            }
            else
            {
                if(req.Password == account.Password)
                {


                    // 토큰 업데이트하거나 추가하는 부분
                    TokenDb tokenDb = _shared.Tokens.Where(t => t.AccountDbId == account.AccountDbId).FirstOrDefault();

                    tokenDb.IsLogin = false;
                    bool success = _shared.SaveChangesEx();

                    if(success)
                    {
                        res.LogoutOk = true;
                    }
                    else
                        res.LogoutOk = false;


                }
                else
                {
                    res.LogoutOk = false;
                }


                
            }


            return res;
        }












    }
}
