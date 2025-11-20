using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.Entities
{
    public class User : IdentityUser<int>
    {

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }

        public int FollowerCount { get; set; } = 0;  // Kaç kişi beni takip ediyor
        public int FollowingCount { get; set; } = 0; // Ben kaç kişiyi takip ediyorum

        // Navigation Properties
        public virtual ICollection<UserMovie> UserMovies { get; set; } = new List<UserMovie>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<MovieList> MovieLists { get; set; } = new List<MovieList>();
        public virtual ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();

        public virtual ICollection<Follow> Followings { get; set; } = new List<Follow>();
        public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();


    }
}
