namespace PhotoStudio.Domain.Models
{
    public enum Role
    {
        Photographer,
        Cameraman,
        Designer,
        Administrator
    }

    public class Employee
    {
        public int Id { get; set; }
        public string JMBG { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }
        public ICollection<EmployeeBooking> EmployeeBookings { get; set; }
        public ICollection<Media> Media { get; set; }

    }
}
