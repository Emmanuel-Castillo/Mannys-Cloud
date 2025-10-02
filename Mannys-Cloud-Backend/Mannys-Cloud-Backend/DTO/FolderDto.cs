namespace Mannys_Cloud_Backend.DTO
{
    public class FolderDto
    {
        public required int FolderId { get; set; }

        public required int UserId { get; set; }

        public required string FolderName { get; set; }

        public required bool IsRootFolder { get; set; }

        public required int? ParentFolderId { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required bool IsDeleted { get; set; }

        public required List<FolderDto> ChildFolders { get; set; }

        public required List<FileDto> Files { get; set; }
    }
}
