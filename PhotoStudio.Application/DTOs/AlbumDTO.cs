

namespace PhotoStudio.Application.DTOs
{
    public class AlbumDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public int EmployeeId { get; set; }
        public int? UserId { get; set; }
        public string FirstName { get; set; }  // Ime korisnika
        public string LastName { get; set; }   // Prezime korisnika
        public ICollection<MediaDTO> Media { get; set; }
    }
}
