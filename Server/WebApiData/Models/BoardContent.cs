using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiData.Models
{
    public class BoardContent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public DateTime Date { get; set; }
        public int TotalComment { get; set; }
        public int LikePoint { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
