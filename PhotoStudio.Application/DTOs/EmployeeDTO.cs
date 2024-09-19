using PhotoStudio.Domain.Models;

namespace PhotoStudio.Application.DTOs
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string JMBG { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }

        public ICollection<MediaDTO> Media { get; set; }
    }
}
