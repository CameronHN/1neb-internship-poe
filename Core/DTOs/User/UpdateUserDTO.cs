namespace Portfolio.Core.DTOs.User
{
    public class UpdateUserDTO
    {
        public required string UserId { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
