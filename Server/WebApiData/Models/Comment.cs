using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiData.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int BoardContentId { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public DateTime Date { get; set; }
        public int LikePoint { get; set; }
    }
}
