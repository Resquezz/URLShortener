using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using URLShortener.Domain.Models;

namespace URLShortener.Infrastructure.Data
{
    public class ShortenerDbContext : DbContext
    {
        public ShortenerDbContext(DbContextOptions<ShortenerDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ShortenedURL> ShortenedURLs { get; set; } = null!;
        public DbSet<About> Abouts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShortenedURL>()
                .HasIndex(u => u.LongURL)
                .IsUnique();

            modelBuilder.Entity<ShortenedURL>()
                .HasIndex(u => u.ShortCode)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
