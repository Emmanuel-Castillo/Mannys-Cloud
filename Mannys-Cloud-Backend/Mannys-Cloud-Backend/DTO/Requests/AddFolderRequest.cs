namespace Mannys_Cloud_Backend.DTO.Requests
{
    public class AddFolderRequest
    {
        public required int UserId { get; set; }

        public required string FolderName { get; set; }

        public required int ParentFolderId { get; set; }

    }
}
