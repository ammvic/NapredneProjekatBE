using MongoDB.Bson;
using MongoDB.Driver;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoStudio.Infrastructure.MongoDB
{
    public class MongoService
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _database;

        public MongoService(IMongoClient client)
        {
            _mongoClient = client;
            _database = client.GetDatabase("photostudio");
        }

        public async Task InitializeDatabaseAsync()
        {
            var collections = await _database.ListCollectionNamesAsync();
            var collectionNames = await collections.ToListAsync();

            var requiredCollections = new List<string>
            {
               "albums", "bookings", "payments",
                "users", "employees"
            };

            foreach (var collection in requiredCollections)
            {
                if (!collectionNames.Contains(collection))
                {
                    await _database.CreateCollectionAsync(collection);
                }
            }

            await SeedDataAsync();
        }

        private async Task SeedDataAsync()
        {
            var usersCollection = _database.GetCollection<User>("users");
            if (await usersCollection.CountDocumentsAsync(new BsonDocument()) == 0)
            {
                await SeedUsersAsync(usersCollection);
            }

            var employeesCollection = _database.GetCollection<Employee>("employees");
            if (await employeesCollection.CountDocumentsAsync(new BsonDocument()) == 0)
            {
                await SeedEmployeesAsync(employeesCollection);
            }


            var bookingsCollection = _database.GetCollection<Booking>("bookings");
            if (await bookingsCollection.CountDocumentsAsync(new BsonDocument()) == 0)
            {
                await SeedBookingsAsync(bookingsCollection);
            }



            var paymentsCollection = _database.GetCollection<Payment>("payments");
            if (await paymentsCollection.CountDocumentsAsync(new BsonDocument()) == 0)
            {
                await SeedPaymentsAsync(paymentsCollection);
            }


            var albumsCollection = _database.GetCollection<Album>("albums");
            if (await albumsCollection.CountDocumentsAsync(new BsonDocument()) == 0)
            {
                await SeedAlbumsAsync(albumsCollection);
            }
        }

        private async Task SeedUsersAsync(IMongoCollection<User> usersCollection)
        {
            var users = new List<User>
    {
        new User
        {
            Id = 1,
            FirstName = "Marko",
            LastName = "Petrović",
            Email = "marko.petrović@example.com",
            Password = "hashedpassword1", // Ova vrednost bi trebala da se hashuje
            PhoneNumber = "+381601234567",
            Address = "Beogradska 10, Beograd, Srbija",
            Credits = 100,
            VerificationCode = "ABC123",
            IsVerified = true,
            Bookings = new List<Booking>()
        },
        new User
        {
            Id = 2,
            FirstName = "Jovana",
            LastName = "Jovanović",
            Email = "jovana.jovanović@example.com",
            Password = "hashedpassword2",
            PhoneNumber = "+381601234568",
            Address = "Novosadska 20, Novi Sad, Srbija",
            Credits = 50,
            VerificationCode = "DEF456",
            IsVerified = false,
            Bookings = new List<Booking>()
        },
        new User
        {
            Id = 3,
            FirstName = "Nikola",
            LastName = "Nikolić",
            Email = "nikola.nikolić@example.com",
            Password = "hashedpassword3",
            PhoneNumber = "+381601234569",
            Address = "Niskogorska 30, Niš, Srbija",
            Credits = 75,
            VerificationCode = "GHI789",
            IsVerified = true,
            Bookings = new List<Booking>()
        }
    };

            await usersCollection.InsertManyAsync(users);
        }


        private async Task SeedEmployeesAsync(IMongoCollection<Employee> employeesCollection)
        {
            var employees = new List<Employee>
    {
        new Employee
        {
            Id = 1,
            JMBG = "1234567890123",
            FirstName = "Marko",
            LastName = "Petrović",
            PasswordHash = "hashedpassword1",
            Role = Role.Photographer,
            EmployeeBookings = new List<EmployeeBooking>(),
            Media = new List<Media>()
        },
        new Employee
        {
            Id = 2,
            JMBG = "9876543210987",
            FirstName = "Jovana",
            LastName = "Jovanović",
            PasswordHash = "hashedpassword2",
            Role = Role.Cameraman,
            EmployeeBookings = new List<EmployeeBooking>(),
            Media = new List<Media>()
        },
        new Employee
        {
            Id = 3,
            JMBG = "5678901234567",
            FirstName = "Nikola",
            LastName = "Nikolić",
            PasswordHash = "hashedpassword3",
            Role = Role.Designer,
            EmployeeBookings = new List<EmployeeBooking>(),
            Media = new List<Media>()
        },
        new Employee
        {
            Id = 4,
            JMBG = "1122334455667",
            FirstName = "Ana",
            LastName = "Anić",
            PasswordHash = "hashedpassword4",
            Role = Role.Administrator,
            EmployeeBookings = new List<EmployeeBooking>(),
            Media = new List<Media>()
        }
    };

            await employeesCollection.InsertManyAsync(employees);
        }



        private async Task SeedBookingsAsync(IMongoCollection<Booking> bookingsCollection)
        {
            var bookings = new List<Booking>
    {
        new Booking
        {
            Id = 1,
            UserId = 1,
            ServiceType = "Wedding Photography",
            Location = "Belgrade, Serbia",
            ServiceId = 101,
            DateTime = new DateTime(2024, 9, 15, 14, 0, 0),
            Status = "Confirmed",
            AdvanceAmount = 200.00m,
            UserFirstName = "Marko",
            UserLastName = "Petrović",
            EmployeeId = 1,
            EmployeeFirstName = "Jovana",
            EmployeeLastName = "Jovanović",
            EmployeeBookings = new List<EmployeeBooking>(),
            User = new User
            {
                Id = 1,
                FirstName = "Marko",
                LastName = "Petrović"
            }
        },
        new Booking
        {
            Id = 2,
            UserId = 2,
            ServiceType = "Event Videography",
            Location = "Novi Sad, Serbia",
            ServiceId = 102,
            DateTime = new DateTime(2024, 10, 20, 16, 30, 0),
            Status = "Pending",
            AdvanceAmount = 150.00m,
            UserFirstName = "Jovana",
            UserLastName = "Jovanović",
            EmployeeId = 2,
            EmployeeFirstName = "Marko",
            EmployeeLastName = "Petrović",
            EmployeeBookings = new List<EmployeeBooking>(),
            User = new User
            {
                Id = 2,
                FirstName = "Jovana",
                LastName = "Jovanović"
            }
        },
        new Booking
        {
            Id = 3,
            UserId = 3,
            ServiceType = "Portrait Photography",
            Location = "Niš, Serbia",
            ServiceId = 103,
            DateTime = new DateTime(2024, 11, 5, 10, 0, 0),
            Status = "Cancelled",
            AdvanceAmount = 100.00m,
            UserFirstName = "Nikola",
            UserLastName = "Nikolić",
            EmployeeId = 3,
            EmployeeFirstName = "Ana",
            EmployeeLastName = "Anić",
            EmployeeBookings = new List<EmployeeBooking>(),
            User = new User
            {
                Id = 3,
                FirstName = "Nikola",
                LastName = "Nikolić"
            }
        }
    };

            await bookingsCollection.InsertManyAsync(bookings);
        }

        private async Task SeedPaymentsAsync(IMongoCollection<Payment> paymentsCollection)
        {
            var payments = new List<Payment>
    {
        new Payment
        {
            Id = 1,
            UserId = 201,
            Amount = 199.99m,
            PaymentDate = DateTime.UtcNow.AddDays(-10),
            Status = "Completed",
            User = new User
            {
               
            }
        },
        new Payment
        {
            Id = 2,
            UserId = 202,
            Amount = 299.99m,
            PaymentDate = DateTime.UtcNow.AddDays(-5),
            Status = "Pending",
            User = new User
            {
                
            }
        },
        new Payment
        {
            Id = 3,
            UserId = 203,
            Amount = 149.99m,
            PaymentDate = DateTime.UtcNow,
            Status = "Failed",
            User = new User
            {
               
            }
        }
    };

            await paymentsCollection.InsertManyAsync(payments);
        }




        private async Task SeedAlbumsAsync(IMongoCollection<Album> albumsCollection)
        {
            var albums = new List<Album>
    {
        new Album
        {
            Id = 1,
            Name = "Summer Vacation",
            Code = "SUM2024",
            IsPublic = true,
            EmployeeId = 101,
            UserId = 201,
            CreatedAt = DateTime.UtcNow,
            Media = new List<Media>
            {
                new Media
                {
                    Id = 1,
                    EmployeeId = 101,
                    AlbumId = 1,
                    FilePath = "/media/photos/summer_vacation_1.jpg",
                    MediaType = MediaType.Photo,
                    UploadedAt = DateTime.UtcNow,
                    Cost = 50
                },
                new Media
                {
                    Id = 2,
                    EmployeeId = 101,
                    AlbumId = 1,
                    FilePath = "/media/videos/summer_vacation_1.mp4",
                    MediaType = MediaType.Video,
                    UploadedAt = DateTime.UtcNow,
                    Cost = 150
                }
            },
        },
        new Album
        {
            Id = 2,
            Name = "Wedding Photoshoot",
            Code = "WED2024",
            IsPublic = false,
            EmployeeId = 102,
            UserId = 202,
            CreatedAt = DateTime.UtcNow,
            Media = new List<Media>
            {
                new Media
                {
                    Id = 3,
                    EmployeeId = 102,
                    AlbumId = 2,
                    FilePath = "/media/photos/wedding_1.jpg",
                    MediaType = MediaType.Photo,
                    UploadedAt = DateTime.UtcNow,
                    Cost = 100
                },
                new Media
                {
                    Id = 4,
                    EmployeeId = 103,
                    AlbumId = 2,
                    FilePath = "/media/designs/wedding_invitation.png",
                    MediaType = MediaType.Design,
                    UploadedAt = DateTime.UtcNow,
                    Cost = 200
                }
            },
            
        }
    };

            await albumsCollection.InsertManyAsync(albums);
        }



    }
}
