namespace Mannys_Cloud_Backend.DTO.Requests
{
    public class RegisterUserRequest
    {
        public required string FullName { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }
    }
}
