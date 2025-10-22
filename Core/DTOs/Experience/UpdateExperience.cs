using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Experience
{
    public class UpdateExperience: AddExperience
    {
        [Required]
        public Guid Id { get; set; }
    }
}
