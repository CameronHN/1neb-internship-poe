using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Certification
{
    public class UpdateCertification : AddCertification
    {
        [Required]
        public Guid Id { get; set; }
    }
}
