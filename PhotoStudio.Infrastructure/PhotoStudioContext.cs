using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhotoStudio.Domain.Models;
using System;
using System.Collections.Generic;

namespace PhotoStudio.Infrastructure
{
    public class PhotoStudioContext : DbContext
    {
        public PhotoStudioContext(DbContextOptions<PhotoStudioContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<EmployeeBooking> EmployeeBookings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite key for EmployeeBooking
            modelBuilder.Entity<EmployeeBooking>()
                .HasKey(eb => new { eb.EmployeeId, eb.BookingId });

            modelBuilder.Entity<EmployeeBooking>()
                .HasOne(eb => eb.Employee)
                .WithMany(e => e.EmployeeBookings)
                .HasForeignKey(eb => eb.EmployeeId);

            modelBuilder.Entity<EmployeeBooking>()
                .HasOne(eb => eb.Booking)
                .WithMany(b => b.EmployeeBookings)
                .HasForeignKey(eb => eb.BookingId);

            // Configure Payment amount column
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            // Configure Media relationships
            modelBuilder.Entity<Media>()
                .HasOne(m => m.Employee)
                .WithMany(e => e.Media)
                .HasForeignKey(m => m.EmployeeId);

            modelBuilder.Entity<Media>()
                .HasOne(m => m.Album)
                .WithMany(a => a.Media)
                .HasForeignKey(m => m.AlbumId);

            // Seed data
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Password = "hashed_password", PhoneNumber = "1234567890", Address = "123 Elm Street", Credits = 100, VerificationCode = "123456", IsVerified = true }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, JMBG = "1234567890123", FirstName = "Amina", LastName = "Mem", Email="ammvic11@gmail.com", PasswordHash = "30730fae9c74c5cee52cf0b74364c765b84de1ee0e8cbd6336b52fec8aaa9b22", Role = Role.Administrator }
            );

            modelBuilder.Entity<Booking>().HasData(
                new Booking { Id = 1, UserId = 1, ServiceType = "Wedding", Location = "Venue A", ServiceId = 1, DateTime = new DateTime(2024, 12, 1, 14, 0, 0), Status = "Confirmed", AdvanceAmount = 50.00m, UserFirstName = "John", UserLastName = "Doe", EmployeeId = 1, EmployeeFirstName = "Jane", EmployeeLastName = "Smith", IsAdditionalShootingIncluded = false }
            );

            modelBuilder.Entity<Album>().HasData(
                new Album { Id = 1, Name = "Wedding Album", Code = "WED123", IsPublic = false, EmployeeId = 1, UserId = 1, CreatedAt = DateTime.UtcNow }
            );

            modelBuilder.Entity<Media>().HasData(
                new Media { Id = 1, EmployeeId = 1, AlbumId = 1, FilePath = "photos/photo1.jpg", MediaType = MediaType.Photo, UploadedAt = DateTime.UtcNow, Cost = 10 },
                new Media { Id = 2, EmployeeId = 1, AlbumId = 1, FilePath = "photos/photo2.jpg", MediaType = MediaType.Photo, UploadedAt = DateTime.UtcNow, Cost = 15 }
            );

            modelBuilder.Entity<Payment>().HasData(
                new Payment { Id = 1, UserId = 1, Amount = 50.00m, PaymentDate = DateTime.UtcNow, Status = "Completed" }
            );

            modelBuilder.Entity<EmployeeBooking>().HasData(
                new EmployeeBooking { BookingId = 1, EmployeeId = 1, Role = Role.Photographer }
            );
        }


    }
}
