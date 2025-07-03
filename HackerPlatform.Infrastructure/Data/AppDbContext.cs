using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackerPlatform.Core.Entities;
using HackerPlatform.Core.Entities.E_Commerce;
using HackerPlatform.Core.Entities.Questions;
using Microsoft.EntityFrameworkCore;

namespace HackerPlatform.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets_HackerPlatform
        public DbSet<User> Users => Set<User>();
        public DbSet<Level> Levels => Set<Level>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<Vulnerability> Vulnerabilities => Set<Vulnerability>();
        public DbSet<Achievement> Achievements => Set<Achievement>();
        public DbSet<LevelProgress> LevelProgresses => Set<LevelProgress>();

        //Quetions
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();

        //E-Commerce
        public DbSet<ECommerceUser> ECommerceUsers => Set<ECommerceUser>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
        public DbSet<UserSession> UserSessions { get; set; }

        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
        public DbSet<Coupon> Coupons => Set<Coupon>();



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ممكن تضيف علاقات خاصة هنا
            modelBuilder.Entity<Achievement>()
                .HasOne(a => a.User)
                .WithMany(u => u.Achievements)
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<Achievement>()
                .HasOne(a => a.Module)
                .WithMany()
                .HasForeignKey(a => a.ModuleId);

            modelBuilder.Entity<LevelProgress>()
                .HasOne(p => p.User)
                .WithMany(u => u.LevelProgresses)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<LevelProgress>()
                .HasOne(p => p.Level)
                .WithMany()
                .HasForeignKey(p => p.LevelId);

            modelBuilder.Entity<Product>().HasData(
    new Product { Id = 1, Name = "Wireless Headphones", Description = "High quality", Price = 200, ImageFileName = "Phone" },
    new Product { Id = 2, Name = "Gaming Mouse", Description = "RGB and programmable", Price = 150, ImageFileName = "Mic" },
    new Product { Id = 3, Name = "Mechanical Keyboard", Description = "Clicky switches", Price = 300, ImageFileName = "Wire" }
);

        }
    }

}
