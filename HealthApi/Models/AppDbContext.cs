﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetTopologySuite;



namespace HealthApi.Models
{
    
    public class AppDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<TestRecord> TestRecords { get; set; }
        public DbSet<VaccineRecord> VaccineRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PermitRequest> PermitRequests { get; set; }
        public DbSet<SelfAssessment> SelfAssessments { get; set; }
        public DbSet<EPass> EPasses { get; set; }
        public DbSet<AnonymousCheckIn> AnonymousCheckIns { get; set; }
        public DbSet<RiskArea> RiskAreas { get; set; }
        public DbSet<QuarantineLocation> QuarantineLocations { get; set; }
        public DbSet<UserLocation> UserLocations { get; set; }
        public DbSet<TestType> TestTypes { get; set; }
        public DbSet<VaccineType> VaccineTypes { get; set; }
        public DbSet<Lab> Labs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Place> Places { get; set; }


        public AppDbContext() : base() { }
        public AppDbContext(DbContextOptions options) : base(options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-1H9P87A\\SQLEXPRESS;Database=TestHealthDb;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False",
                x => x.UseNetTopologySuite());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // علاقات اختيارية/توضيحية

            modelBuilder.Entity<UserLocation>()
            .HasOne(ul => ul.User)           // UserLocation له User واحد
            .WithOne(u => u.UserLocation)   // والـ User ده له UserLocation واحد
            .HasForeignKey<UserLocation>(ul => ul.UserId); // المفتاح الأجنبي موجود في UserLocation وهو الـ UserId

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.TargetArea)
                .WithMany()
                .HasForeignKey(n => n.TargetAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.TatgetUser)
                .WithMany()
                .HasForeignKey(n => n.TatgetUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RiskArea>()
                .Property(a => a.Geometry)
                .HasColumnType("geometry");
        }
    }

}
