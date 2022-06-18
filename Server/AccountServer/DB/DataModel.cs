using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AccountServer.DB
{

    // Account Server 용 DataModel

    [Table("Account")]
    public class AccountDb   // Enity라는걸 알리기 위해 Db를 붙인다.
    {
        public int AccountDbId { get; set; } // Id를 넣으면 주키가 된다고함.
        public string AccountName { get; set; } // (Unique) 보통 유저가 게임시작할때 입력하거나, 휴대폰의 기기값을 여기에 넣는다.
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Nickname { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }
        public string Email { get; set; }

    }
}
