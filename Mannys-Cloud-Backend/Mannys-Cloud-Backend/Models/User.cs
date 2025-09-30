namespace Mannys_Cloud_Backend.Models
{
    public class User
    {
        public int UserId { get; set; }

        public required string FullName { get; set; }

        public required string Email { get; set; }

        public required string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Folder> UserFolders { get; set; } = new List<Folder>();
        public ICollection<File> UserFiles { get; set; } = new List<File>();



    }
}
