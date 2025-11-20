using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.Entities
{
    // film listelerindeki filmleri temsil eder
    public class ListMovie : BaseEntity
    {
        public int ListId { get; set; }
        public int MovieId { get; set; }
        public int Order { get; set; } // Liste içindeki sıra

        public virtual MovieList MovieList { get; set; } = null!;
        public virtual Movie Movie { get; set; } = null!;
    }
}
