namespace Portfolio.Core.DTOs
{
    public class CertificationRequest
    {
        public List<Guid> Ids { get; set; } = [];
        public bool IsDescending { get; set; } = false;

    }
}
