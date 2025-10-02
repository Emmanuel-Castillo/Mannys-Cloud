using System.ComponentModel.DataAnnotations.Schema;

namespace Mannys_Cloud_Backend.Models
{
    public class File
    {
        public int FileId { get; set; }

        [ForeignKey("Users")]
        public required int UserId { get; set; }                // User who owns the file

        public required string FileName { get; set; }           // Original file name

        public required string BlobPath { get; set; }           // Path in Azure Blob Storage (container + blob name)

        public required string ContentType { get; set; }        // MIME type (e.g., application/pdf, image/png)

        public required int SizeBytes { get; set; }             // Size of file (bytes)

        [ForeignKey("Folder")]
        public required int FolderId { get; set; }              // Parent folder

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int IsDeleted { get; set; } = 0;

        // Navigation
        public User User { get; set; } = null!;
        public Folder Folder { get; set; } = null!;
    }
}
