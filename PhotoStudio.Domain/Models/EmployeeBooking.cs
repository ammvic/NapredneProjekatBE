using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoStudio.Domain.Models
{
    public class EmployeeBooking
    {
        public int BookingId { get; set; }
        public Booking Booking { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        // Opcionalno: dodatne informacije o ulozi zaposlenog u rezervaciji
        public Role Role { get; set; }
    }


}
