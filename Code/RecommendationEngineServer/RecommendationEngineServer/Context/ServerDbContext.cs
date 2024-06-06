using Microsoft.EntityFrameworkCore;
using RecommendationEngineServer.Models.Entities;

namespace RecommendationEngineServer.Context
{
    public class ServerDbContext : DbContext
    {
        public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(f => f.Role)
                .WithMany()
                .HasForeignKey(f => f.RoleId);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Menu)
                .WithMany()
                .HasForeignKey(f => f.MenuId);

            modelBuilder.Entity<Recommendation>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Recommendation>()
                .HasOne(f => f.Menu)
                .WithMany()
                .HasForeignKey(f => f.MenuId);

            modelBuilder.Entity<Vote>()
               .HasOne(v => v.User)
               .WithMany()
               .HasForeignKey(v => v.UserId);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Recommendation)
                .WithMany()
                .HasForeignKey(v => v.RecommendationId);

            modelBuilder.Entity<Employee>()
               .HasOne(v => v.User)
               .WithMany()
               .HasForeignKey(v => v.UserId);

            modelBuilder.Entity<Notification>()
               .HasOne(v => v.User)
               .WithMany()
               .HasForeignKey(v => v.UserId);
        }
    }
}
