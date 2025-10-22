using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Education
{
    public class UpdateEducation : AddEducation
    {
        [Required]
        public Guid Id { get; set; }
    }
}
