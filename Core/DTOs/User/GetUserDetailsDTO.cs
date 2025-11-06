namespace Portfolio.Core.DTOs.User
{
    public class GetUserDetailsDTO
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public required string PhoneNumber { get; set; }
    }
}
