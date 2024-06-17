using Microsoft.EntityFrameworkCore;
using RecommendationEngineServer.Models.Entities;

namespace RecommendationEngineServer.Context
{
    public class ServerDbContext : DbContext
    {
        public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<FoodItem> FoodItems { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<RecommendedMenu> RecommendedMenus { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Data Source=ITT-VAISHAK-S;Database=RecommendationEngine1;Trusted_Connection=True")
                .UseLazyLoadingProxies();
        }

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
                .HasOne(f => f.FoodItem)
                .WithMany()
                .HasForeignKey(f => f.FoodItemId);

            modelBuilder.Entity<RecommendedMenu>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<RecommendedMenu>()
                .HasOne(f => f.FoodItem)
                .WithMany()
                .HasForeignKey(f => f.FoodItemId);

            modelBuilder.Entity<Order>()
               .HasOne(v => v.User)
               .WithMany()
               .HasForeignKey(v => v.UserId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(v => v.RecommendedMenu)
                .WithMany()
                .HasForeignKey(v => v.MenuId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(v => v.Order)
                .WithMany()
                .HasForeignKey(v => v.OrderId);

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
