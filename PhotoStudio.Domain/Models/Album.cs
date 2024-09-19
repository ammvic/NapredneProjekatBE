namespace PhotoStudio.Domain.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }  // Korisnici će koristiti ovo za pristup
        public bool IsPublic { get; set; }  // Određuje da li je album javno dostupan
        public int EmployeeId { get; set; }
        public int? UserId { get; set; }  // Referenca na korisnika
        public DateTime CreatedAt { get; set; }

        public ICollection<Media> Media { get; set; }
        public User User { get; set; }  // Navigaciono svojstvo za korisnika
    }
}
