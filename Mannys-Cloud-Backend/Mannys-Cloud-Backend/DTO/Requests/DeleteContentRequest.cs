namespace Mannys_Cloud_Backend.DTO.Requests
{
    public class DeleteContentRequest
    {
        public required List<int> FolderIds { get; set; }

        public required List<int> FileIds { get; set; }
    }
}
