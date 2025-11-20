using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.Entities
{
    public class Review : BaseEntity
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }

        public string Content { get; set; } = string.Empty;
        public int LikeCount { get; set; } = 0;

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Movie Movie { get; set; } = null!;
    }
}
