using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Src.Models;

namespace Src.Data
{
    public class ApplicationDBContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Actives> Actives { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Gọi base để đảm bảo cấu hình mặc định của Identity được áp dụng

            modelBuilder.Entity<Post>(e =>
            {
                e.HasKey(p => p.PostID);
                e.HasOne(e => e.Category)
                    .WithMany(c => c.Posts)
                   .HasForeignKey(p => p.CategoryID);

                e.HasOne(e => e.Tag)
                    .WithMany(t => t.Posts)
                   .HasForeignKey(p => p.TagID);

                e.Property(p => p.PublishedDate).HasDefaultValueSql("getutcdate()");
            });

            modelBuilder.Entity<Category>(e =>
            {
                e.HasKey(c => c.CategoryID);

            });

            modelBuilder.Entity<Comment>(e =>
            {
                e.HasKey(cmt => new { cmt.CommentId });
                
                e.HasOne(cmt => cmt.Post)
                   .WithMany(p => p.Comments)
                   .HasForeignKey(cmt => cmt.PostId);

                e.HasOne(cmt => cmt.AppUser)
                   .WithMany(u => u.Comments)
                   .HasForeignKey(cmt => cmt.AppUserID);
            });

            modelBuilder.Entity<Tag>(e =>
            {
                e.HasKey(t => t.TagID);
                e.HasOne(t => t.Category)
                .WithMany(c => c.Tags)
                   .HasForeignKey(t => t.CategoryID);
    
            });

            modelBuilder.Entity<Actives>(e =>
            {
                e.HasKey(a => a.ActivesID);
                e.HasOne(a => a.AppUser)
                   .WithMany(u => u.IsActive)
                   .HasForeignKey(a => a.AppUserID);
            });

            List<IdentityRole> roles = new()
            {
                new IdentityRole {Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole {Name = "User", NormalizedName = "USER" },
                new IdentityRole {Name = "Employee", NormalizedName = "EMPLOYEE" }

            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);

        }
    }
}
