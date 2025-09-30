namespace Portfolio.Core.DTOs
{
    public class UpdateUserDTO
    {
        public required string UserId { get; set; }

        public string? Gender { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
