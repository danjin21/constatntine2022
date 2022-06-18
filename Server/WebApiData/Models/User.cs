using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiData.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Nickname { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }
    }
}
