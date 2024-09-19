
using System;

namespace PhotoStudio.Application.DTOs
{
    public class BookingDTO
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
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public bool IsAdditionalShootingIncluded { get; set; }
    }
}
