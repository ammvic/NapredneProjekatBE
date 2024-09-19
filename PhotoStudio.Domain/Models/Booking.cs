using System;

namespace PhotoStudio.Domain.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ServiceType { get; set; }
        public string Location { get; set; }
        public int ServiceId { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public decimal AdvanceAmount { get; set; }

        // Podaci o korisniku
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }

        // Podaci o zaposlenom (ime i prezime)
        public int EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public ICollection<EmployeeBooking> EmployeeBookings { get; set; }
        // Opcionalno polje za označavanje da li je snimanje uključeno
        public bool IsAdditionalShootingIncluded { get; set; }

        public User User { get; set; }
    }
}
