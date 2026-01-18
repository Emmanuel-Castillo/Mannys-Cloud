using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Models;
using Mannys_Cloud_Backend.Util;
using Microsoft.EntityFrameworkCore;

namespace Mannys_Cloud_Backend.Services
{
    public class FolderService : IFolderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobStorage _blobStorage;
        public FolderService(ApplicationDbContext context, IBlobStorage blobStorage)
        {
            _context = context;
            _blobStorage = blobStorage;
        }
        public async Task<int> AddFolder(AddFolderRequest request)
        {
            try {
                // Validate request UserId and ParentFolderId existence
                var user = await _context.Users.FindAsync(request.UserId);
                var parentFolder = await _context.Folders.FindAsync(request.ParentFolderId);

                if (user == null || parentFolder == null) throw new Exception("User or Parent Folder not valid.");

                var newFolder = new Folder
                {
                    UserId = request.UserId,
                    FolderName = request.FolderName,
                    ParentFolderId = request.ParentFolderId,
                };

                _context.Folders.Add(newFolder);
                await _context.SaveChangesAsync();

                return newFolder.FolderId;
            }
            catch {
                throw;
            }
        }

        public async Task DeleteSingleFolder(int folderId)
        {
            try { 
                var folder = await _context.Folders.FindAsync(folderId);
                if (folder == null) throw new Exception("Folder not found!");

                await DeleteFolderContents(folder);
                await _context.SaveChangesAsync();
            }
            catch {
                throw;
            }
        }

        public async Task DeleteMultipleFolders(List<int> folderIds)
        {
            try {
                var folders = new List<Folder>();
                foreach (var folderId in folderIds) { 
                    var f = await _context.Folders.FindAsync(folderId);
                    if (f == null) throw new Exception ("Folder not found!");
                    folders.Add(f);
                }
                var tasks = folders.Select(id => DeleteFolderContents(id));
                await Task.WhenAll(tasks);
                await _context.SaveChangesAsync();
            }
            catch {
                throw;
            }
        }

        public async Task<FolderDto> GetFolder(int folderId)
        {
            try {
                var folder = await _context.Folders
                    .AsNoTracking()     // Important! Prevents deleted content from being part of result
                    .Include(f => f.FolderFiles.Where(ff => !ff.IsDeleted))
                    .Include(f => f.ChildFolders.Where(cf => !cf.IsDeleted))
                    .FirstOrDefaultAsync(f => f.FolderId == folderId);

                if (folder == null) throw new Exception("Folder not found!");

                var folderDto = ConvertToDto.ConvertToFolderDto(folder);
                return folderDto;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<FolderDto>> GetFolders()
        {
            try {
                var folders = await _context.Folders.Include(f => f.FolderFiles).ToListAsync();
                var folderDtos = folders.Select(f => ConvertToDto.ConvertToFolderDto(f)).ToList();
                return folderDtos;
            }
            catch
            {
                throw;
            }
        }

        private async Task DeleteFolderContents(Folder folder)
        {
            // Check if folder is root folder for User
            if (folder.IsRootFolder) throw new Exception("Deleting root folders is prohibited.");
        
            // Grab folder path in Blob Storage namespace
            // Then soft delete the folder in Blob Storage
            var folderPath = BuildPath.BuildFolderPath(folder);
            await _blobStorage.DeleteFolderAsync(folderPath);

            // Set folder and children IsDeleted to true
            var files = folder.FolderFiles.ToList();
            files.ForEach(f => f.IsDeleted = true);

            folder.IsDeleted = true;

            // Recursively delete child folder contents
            folder.ChildFolders.ToList().ForEach(async (f) => await DeleteFolderContents(f));
        }
    }
}
