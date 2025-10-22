using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Core.DTOs.Education
{
    public class UpdateEducation : AddEducation
    {
        [Required]
        public Guid Id { get; set; }
    }
}
