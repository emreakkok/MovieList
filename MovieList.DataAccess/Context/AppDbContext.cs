using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieList.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.DataAccess.Context
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<UserMovie> UserMovies { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<MovieLike> MovieLikes { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Core.Entities.MovieList> MovieLists { get; set; }
        public DbSet<ListMovie> ListMovies { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Bu tablolar BaseEntity'den miras alsa bile Id'yi anahtar yapmasını engelliyoruz:
            modelBuilder.Entity<Watchlist>().HasKey(w => new { w.UserId, w.MovieId });
            modelBuilder.Entity<MovieLike>().HasKey(ml => new { ml.UserId, ml.MovieId });
            modelBuilder.Entity<ListMovie>().HasKey(lm => new { lm.ListId, lm.MovieId });
            modelBuilder.Entity<UserMovie>().HasKey(um => new { um.UserId, um.MovieId });
            modelBuilder.Entity<Follow>().HasKey(f => new { f.FollowerId, f.FollowingId });

            // Takip Eden (Follower) tarafı
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower) // Follow kaydında kim takip ediyor? (FollowerId)
                .WithMany(u => u.Followings) // Bu kullanıcı tarafından başlatılan takip kayıtları (Benim takip ettiklerim)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Takip Edilen (Following) tarafı
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Following) // Follow kaydında kim takip ediliyor? (FollowingId)
                .WithMany(u => u.Followers) // Bu kullanıcı tarafından bitirilen takip kayıtları (Beni takip edenler)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
