using System.ComponentModel.DataAnnotations.Schema;

namespace Mannys_Cloud_Backend.Models
{
    public class Folder
    {
        public int FolderId { get; set; }

        [ForeignKey("User")]
        public required int UserId { get; set; }

        public required string FolderName { get; set; }

        public bool IsRootFolder { get; set; }

        [ForeignKey("Folder")]
        public int? ParentFolderId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        // Navigation
        public User User { get; set; } = null!;
        public ICollection<File> FolderFiles { get; set; } = new List<File>();

        public ICollection<Folder> ChildFolders { get; set; } = new List<Folder>();
        public Folder ParentFolder { get; set; } = null!;
    }
}
