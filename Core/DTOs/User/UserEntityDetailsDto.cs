namespace Portfolio.Core.DTOs
{
    public class UserEntityDetailsDto
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? Gender { get; set; }

        public string? PhoneNumber { get; set; }

        public required string Email { get; set; }
    }
}
