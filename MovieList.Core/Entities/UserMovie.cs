using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.Entities
{
    // Kullanıcının izlediği filmlerle ilgili bilgileri tutar
    public class UserMovie : BaseEntity
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }

        public bool IsWatched { get; set; } = true;
        public DateTime? WatchedDate { get; set; }
        public int? Rating { get; set; } // 1-5 arası
        public bool IsFavorite { get; set; } = false;

        public virtual User User { get; set; } = null!;
        public virtual Movie Movie { get; set; } = null!;
    }
}
