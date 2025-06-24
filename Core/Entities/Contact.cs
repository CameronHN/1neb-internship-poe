using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.Entities
{
    public class Contact
    {
        [Key]
        public Guid Id { get; set; }

        public string LinkedIn { get; set; } = string.Empty;

        public string GitHub { get; set; } = string.Empty;

        public Guid UserId { get; set; }

        public required User User { get; set; }

    }
}
