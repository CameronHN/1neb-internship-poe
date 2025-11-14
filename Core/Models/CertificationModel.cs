namespace Portfolio.Core.Models
{
    public class CertificationModel
    {
        public Guid Id { get; set; }

        public required string CertificationName { get; set; }

        public string? IssuingOrganisation { get; set; }

        public string? CredentialUrl { get; set; }

        public DateOnly? IssuedDate { get; set; }

        public DateOnly? ExpiryDate { get; set; }
    }
}
