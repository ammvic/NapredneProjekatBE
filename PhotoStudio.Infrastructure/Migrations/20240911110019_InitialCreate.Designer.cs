﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PhotoStudio.Infrastructure;

#nullable disable

namespace PhotoStudio.Infrastructure.Migrations
{
    [DbContext(typeof(PhotoStudioContext))]
    [Migration("20240911110019_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PhotoStudio.Domain.Models.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Albums");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Code = "WED123",
                            CreatedAt = new DateTime(2024, 9, 11, 11, 0, 17, 983, DateTimeKind.Utc).AddTicks(1983),
                            EmployeeId = 1,
                            IsPublic = false,
                            Name = "Wedding Album",
                            UserId = 1
                        });
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("AdvanceAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmployeeFirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("EmployeeLastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAdditionalShootingIncluded")
                        .HasColumnType("bit");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.Property<string>("ServiceType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserFirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("UserLastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Bookings");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AdvanceAmount = 50.00m,
                            DateTime = new DateTime(2024, 12, 1, 14, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeFirstName = "Jane",
                            EmployeeId = 1,
                            EmployeeLastName = "Smith",
                            IsAdditionalShootingIncluded = false,
                            Location = "Venue A",
                            ServiceId = 1,
                            ServiceType = "Wedding",
                            Status = "Confirmed",
                            UserFirstName = "John",
                            UserId = 1,
                            UserLastName = "Doe"
                        });
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JMBG")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Employees");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "ammvic11@gmail.com",
                            FirstName = "Amina",
                            JMBG = "1234567890123",
                            LastName = "Mem",
                            PasswordHash = "30730fae9c74c5cee52cf0b74364c765b84de1ee0e8cbd6336b52fec8aaa9b22",
                            Role = 3
                        });
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.EmployeeBooking", b =>
                {
                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("EmployeeId", "BookingId");

                    b.HasIndex("BookingId");

                    b.ToTable("EmployeeBookings");

                    b.HasData(
                        new
                        {
                            EmployeeId = 1,
                            BookingId = 1,
                            Role = 0
                        });
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.Media", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AlbumId")
                        .HasColumnType("int");

                    b.Property<int>("Cost")
                        .HasColumnType("int");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MediaType")
                        .HasColumnType("int");

                    b.Property<DateTime>("UploadedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("Media");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AlbumId = 1,
                            Cost = 10,
                            EmployeeId = 1,
                            FilePath = "photos/photo1.jpg",
                            MediaType = 0,
                            UploadedAt = new DateTime(2024, 9, 11, 11, 0, 17, 983, DateTimeKind.Utc).AddTicks(2074)
                        },
                        new
                        {
                            Id = 2,
                            AlbumId = 1,
                            Cost = 15,
                            EmployeeId = 1,
                            FilePath = "photos/photo2.jpg",
                            MediaType = 0,
                            UploadedAt = new DateTime(2024, 9, 11, 11, 0, 17, 983, DateTimeKind.Utc).AddTicks(2079)
                        });
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Payments");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Amount = 50.00m,
                            PaymentDate = new DateTime(2024, 9, 11, 11, 0, 17, 983, DateTimeKind.Utc).AddTicks(2170),
                            Status = "Completed",
                            UserId = 1
                        });
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Credits")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VerificationCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Address = "123 Elm Street",
                            Credits = 100,
                            Email = "john.doe@example.com",
                            FirstName = "John",
                            IsVerified = true,
                            LastName = "Doe",
                            Password = "hashed_password",
                            PhoneNumber = "1234567890",
                            VerificationCode = "123456"
                        });
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.Album", b =>
                {
                    b.HasOne("PhotoStudio.Domain.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.Booking", b =>
                {
                    b.HasOne("PhotoStudio.Domain.Models.User", "User")
                        .WithMany("Bookings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.EmployeeBooking", b =>
                {
                    b.HasOne("PhotoStudio.Domain.Models.Booking", "Booking")
                        .WithMany("EmployeeBookings")
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PhotoStudio.Domain.Models.Employee", "Employee")
                        .WithMany("EmployeeBookings")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.Media", b =>
                {
                    b.HasOne("PhotoStudio.Domain.Models.Album", "Album")
                        .WithMany("Media")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PhotoStudio.Domain.Models.Employee", "Employee")
                        .WithMany("Media")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.Payment", b =>
                {
                    b.HasOne("PhotoStudio.Domain.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.Album", b =>
                {
                    b.Navigation("Media");
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.Booking", b =>
                {
                    b.Navigation("EmployeeBookings");
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.Employee", b =>
                {
                    b.Navigation("EmployeeBookings");

                    b.Navigation("Media");
                });

            modelBuilder.Entity("PhotoStudio.Domain.Models.User", b =>
                {
                    b.Navigation("Bookings");
                });
#pragma warning restore 612, 618
        }
    }
}
