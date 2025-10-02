namespace Mannys_Cloud_Backend.DTO.Requests
{
    public class UpdateUserRequest
    {
        public string NewFullName { get; set; } = string.Empty;

        public string NewEmail { get; set; } = string.Empty;
    }
}
