namespace Mannys_Cloud_Backend.DTO.Requests
{
    public class AddFileRequest
    {
        public required IFormFile File { get; set; }

        public required int UserId { get; set; }
        public required int FolderId { get; set; }


    }
}
