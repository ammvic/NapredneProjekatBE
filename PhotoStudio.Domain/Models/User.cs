

namespace PhotoStudio.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int Credits { get; set; }
        public string? VerificationCode { get; set; }
        public bool IsVerified { get; set; } // Dodato za verifikaciju korisnika

        public ICollection<Booking> Bookings { get; set; }
    }
}
