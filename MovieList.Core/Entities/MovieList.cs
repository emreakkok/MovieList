using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.Entities
{
    // kullanıcıların oluşturduğu film listelerini temsil eder
    public class MovieList : BaseEntity
    {
        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPublic { get; set; } = true;


        public virtual User User { get; set; } = null!;
        public virtual ICollection<ListMovie> ListMovies { get; set; } = new List<ListMovie>();
    }
}
