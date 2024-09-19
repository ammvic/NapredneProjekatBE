using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoStudio.Domain.Models;

namespace PhotoStudio.Application.DTOs
{
    public class MediaDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int AlbumId { get; set; }
        public string FilePath { get; set; }
        public MediaType MediaType { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
