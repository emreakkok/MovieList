using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.Entities
{
    public class Follow : BaseEntity
    {
        public int FollowerId { get; set; }
        public int FollowingId { get; set; }


        public virtual User Follower { get; set; } = null!;
        public virtual User Following { get; set; } = null!;
    }
}
