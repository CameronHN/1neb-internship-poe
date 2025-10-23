using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.ProfessionalSummary
{
    public class UpdateSummary : AddSummary
    {
        [Required]
        public Guid Id { get; set; }
    }
}
