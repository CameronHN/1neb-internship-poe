using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Core.DTOs
{
    // TODO: Add validation on DTO
    public class AddUserDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Email { get; set; }

        public string? Gender { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
