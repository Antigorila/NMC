using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NMC.DataAccess.Models;

namespace NMC.DataAccess
{
    public class NmcDbContext : DbContext
    {
        public NmcDbContext(DbContextOptions<NmcDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Track> Tracks => Set<Track>();
        public DbSet<UserTrack> UserTracks => Set<UserTrack>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<GenreScore> GenreScores => Set<GenreScore>();
        public DbSet<AcousticAttribute> AcousticAttributes => Set<AcousticAttribute>();
        public DbSet<AcousticScore> AcousticScores => Set<AcousticScore>();
        public DbSet<Playlist> Playlists => Set<Playlist>();
        public DbSet<PlaylistTrack> PlaylistTracks => Set<PlaylistTrack>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Keys
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.Name)
                .IsUnique();

            modelBuilder.Entity<AcousticAttribute>()
                .HasIndex(a => a.Name)
                .IsUnique();

            modelBuilder.Entity<PlaylistTrack>()
                .HasKey(pt => new { pt.UserTrackId, pt.PlaylistId });
            #endregion

            #region Relations
            // UserTrack -> Track & User
            modelBuilder.Entity<UserTrack>()
                .HasOne(ut => ut.Track)
                .WithMany(t => t.UserTracks)
                .HasForeignKey(ut => ut.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserTrack>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTracks)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // GenreScore -> Track & Genre
            modelBuilder.Entity<GenreScore>()
                .HasOne(gs => gs.Track)
                .WithMany(t => t.GenreScores)
                .HasForeignKey(gs => gs.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GenreScore>()
                .HasOne(gs => gs.Genre)
                .WithMany(g => g.GenreScores)
                .HasForeignKey(gs => gs.GenreId)
                .OnDelete(DeleteBehavior.Cascade);

            // AcousticScore -> Track & AcousticAttribute
            modelBuilder.Entity<AcousticScore>()
                .HasOne(a => a.Track)
                .WithMany(t => t.AcousticScores)
                .HasForeignKey(a => a.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AcousticScore>()
                .HasOne(a => a.Attribute)
                .WithMany(a => a.AcousticScores)
                .HasForeignKey(a => a.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Playlist -> User
            modelBuilder.Entity<Playlist>()
                .HasOne(p => p.User)
                .WithMany(u => u.Playlists)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // PlaylistTrack -> UserTrack & Playlist
            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(pt => pt.UserTrack)
                .WithMany(ut => ut.PlaylistTracks)
                .HasForeignKey(pt => pt.UserTrackId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(pt => pt.Playlist)
                .WithMany(p => p.PlaylistTracks)
                .HasForeignKey(pt => pt.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
