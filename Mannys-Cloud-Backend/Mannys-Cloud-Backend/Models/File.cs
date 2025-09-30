using System.ComponentModel.DataAnnotations.Schema;

namespace Mannys_Cloud_Backend.Models
{
    public class File
    {
        public int FileId { get; set; }

        [ForeignKey("Users")]
        public required int UserId { get; set; }

        public required string FileName { get; set; }

        public required string BlobPath { get; set; }

        public required string ContentType { get; set; }

        public required int SizeBytes { get; set; }

        [ForeignKey("Folder")]
        public int FolderId { get; set; }

        public required DateTime CreatedAt { get; set; } = DateTime.Now;

        public required int IsDeleted { get; set; }

        // Navigation
        public User User { get; set; } = null!;
        public Folder Folder { get; set; } = null!;
    }
}
