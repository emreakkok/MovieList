using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.DTOs.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }
        public int MovieId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
