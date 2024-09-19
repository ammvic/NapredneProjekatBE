using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoStudio.Application.DTOs
{
    public class VerifyRequest
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
    }
}
