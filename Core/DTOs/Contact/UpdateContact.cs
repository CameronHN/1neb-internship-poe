using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Contact
{
    public class UpdateContact: AddContact
    {
        [Required]
        public Guid Id { get; set; }
    }
}
