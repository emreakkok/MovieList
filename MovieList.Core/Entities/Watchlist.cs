using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.Entities
{
    public class Watchlist : BaseEntity
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }

        public DateTime AddedDate { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Movie Movie { get; set; } = null!;
    }
}
