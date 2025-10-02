namespace Mannys_Cloud_Backend.DTO
{
    public class FileDto
    {
        public required int FileId { get; set; }

        public required int UserId { get; set; }

        public required string FileName { get; set; }

        public required string BlobPath { get; set; }

        public required string ContentType { get; set; }

        public required int SizeBytes { get; set; }

        public required int? FolderId { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required bool IsDeleted { get; set; }
    }
}
