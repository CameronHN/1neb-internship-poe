namespace Portfolio.Core.DTOs
{
    public class GetUserDetailsDTO
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Gender { get; set; }
    }
}
