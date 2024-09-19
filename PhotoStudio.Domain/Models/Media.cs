using System;

namespace PhotoStudio.Domain.Models
{
    public enum MediaType
    {
        Photo,
        Video,
        Design
    }

    public class Media
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int AlbumId { get; set; }
        public string FilePath { get; set; }
        public MediaType MediaType { get; set; }
        public DateTime UploadedAt { get; set; }
        public int Cost { get; set; }

        // Navigacijska svojstva
        public Employee Employee { get; set; }
        public Album Album { get; set; }
    }
}
