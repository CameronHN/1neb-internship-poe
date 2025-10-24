using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.ResumeTitle
{
    public class UpdateResumeTitle : AddResumeTitle
    {
        [Required]
        public Guid Id { get; set; }
    }
}
