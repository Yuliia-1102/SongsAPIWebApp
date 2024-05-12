﻿using Microsoft.EntityFrameworkCore;

namespace SongsAPIWebApp.Models
{
    public partial class SongsAPIContext : DbContext
    {
        public virtual DbSet<Singer> Singers { get; set; }
        public virtual DbSet<SingersSong> SingersSongs { get; set; }
        public virtual DbSet<Song> Songs { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public SongsAPIContext(DbContextOptions<SongsAPIContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<SingersSong>(entity =>
            {
                entity.HasKey(e => new { e.SingerId, e.SongId });

                entity.HasOne(d => d.Singer)
                      .WithMany(p => p.SingersSongs)
                      .HasForeignKey(d => d.SingerId);

                entity.HasOne(d => d.Song)
                      .WithMany(p => p.SingersSongs)
                      .HasForeignKey(d => d.SongId);
            });

            modelBuilder.Entity<Song>(entity =>
            {
                entity.HasOne(d => d.Genre)
                      .WithMany(p => p.Songs)
                      .HasForeignKey(d => d.GenreId);
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(e => new { e.CustomerId, e.SongId });

                entity.HasOne(d => d.Customer)
                      .WithMany(p => p.Purchases)
                      .HasForeignKey(d => d.CustomerId);

                entity.HasOne(d => d.Song)
                      .WithMany(p => p.Purchases)
                      .HasForeignKey(d => d.SongId);
            });
        }
    }
}
