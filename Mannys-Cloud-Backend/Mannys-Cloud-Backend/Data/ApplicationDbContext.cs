using Mannys_Cloud_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Mannys_Cloud_Backend.Data
{
    public class ApplicationDbContext: DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Models.File> Files { get; set; }
        public DbSet<Folder> Folders { get; set; }  

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.File>()
                .HasOne(f => f.User)
                .WithMany(u => u.UserFiles)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.File>()
                .HasOne(f => f.Folder)
                .WithMany(f => f.FolderFiles)
                .HasForeignKey(f => f.FolderId)
                .OnDelete(DeleteBehavior.Restrict);

            // ParentFolder relationship (no cascade at DB level)
            modelBuilder.Entity<Folder>()
                .HasOne(f => f.ParentFolder)
                .WithMany(f => f.ChildFolders)
                .HasForeignKey(f => f.ParentFolderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Folder>()
                .HasOne(f => f.User)
                .WithMany(u => u.UserFolders)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
