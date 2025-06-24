using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.Entities
{
    public class ProfessionalSummary
    {
        [Key]
        public Guid Id { get; set; }

        public string? Summary { get; set; }

        public Guid UserId { get; set; }

        public required User User { get; set; }
    }
}
