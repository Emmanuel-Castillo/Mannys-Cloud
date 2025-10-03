using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.Models;

namespace Mannys_Cloud_Backend.Util
{
    public class ConvertDto
    {
        public FileDto ConvertToFileDto(Models.File file)
        {
            return new FileDto
            {
                UserId = file.UserId,
                FileName = file.FileName,
                BlobPath = file.BlobPath,
                ContentType = file.ContentType,
                CreatedAt = file.CreatedAt,
                FileId = file.FileId,
                FolderId = file.FolderId,
                IsDeleted = file.IsDeleted,
                SizeBytes = file.SizeBytes,
            };
        }

        public FolderDto ConvertToFolderDto(Folder folder)
        {
            return new FolderDto
            {
                CreatedAt = folder.CreatedAt,
                FolderId = folder.FolderId,
                FolderName = folder.FolderName,
                IsDeleted = folder.IsDeleted,
                IsRootFolder = folder.IsRootFolder,
                ParentFolderId = folder.ParentFolderId,
                UserId = folder.UserId,
                ChildFolders = folder.ChildFolders.Select(f => this.ConvertToFolderDto(f)).ToList(),
                Files = folder.FolderFiles.Select(f => this.ConvertToFileDto(f)).ToList(),
            };
        }

        public UserDto ConvertToUserDto(User user) {
            return new UserDto
            {
                Email = user.Email,
                FullName = user.FullName,
                UserId = user.UserId,
            };
        }
    }
}
