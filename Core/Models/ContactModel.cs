namespace Portfolio.Core.Models
{
    public class ProfessionalLinkModel
    {
        public Guid Id { get; set; }
        public required string LinkType { get; set; }

        public required string Link { get; set; }
    }
}
