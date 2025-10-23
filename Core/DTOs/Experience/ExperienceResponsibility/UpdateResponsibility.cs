using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Experience.ExperienceResponsibility
{
    public class UpdateResponsibility : AddResponsibility
    {
        [Required]
        public Guid Id { get; set; }
    }
}
