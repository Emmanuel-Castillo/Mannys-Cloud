using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Models;
using Mannys_Cloud_Backend.Util;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Mannys_Cloud_Backend.Services
{
    public record UserTrashContent
    {
        public List<FileDto> trashFiles;
        public List<FolderDto> trashFolders;
        public UserTrashContent(List<FileDto> files, List<FolderDto> folders)
        {
            trashFiles = files;
            trashFolders = folders;
        }
    }

    public record UserData
    {
        public UserDto userDto;
        public ICollection<Models.File> userFiles;
        public ICollection<Folder> userFolders;

        public UserData(UserDto userDto, ICollection<Models.File> userFiles1, ICollection<Folder> userFolders)
        {
            this.userDto = userDto;
            this.userFiles = userFiles1;
            this.userFolders = userFolders;
        }
    }
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DeleteUser(int userId)
        {
            try { 
                var user = await _context.Users.FindAsync(userId);
                if (user == null) throw new Exception("User not found!");

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch {
                throw;
            }
        }

        public async Task<UserData> GetUser(int userId)
        {
            try {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) throw new Exception("User not found!");

                return new UserData(ConvertToDto.ConvertToUserDto(user), user.UserFiles, user.UserFolders);
            }
            catch {
                throw;
            }
        }

        public async Task<FolderDto> GetUserRootFolder(int userId)
        {
            try {var rootFolder = await _context.Folders.Include(f => f.FolderFiles.Where(ff => ff.IsDeleted == false)).Include(f => f.ChildFolders.Where(cf => cf.IsDeleted == false)).FirstAsync(f => f.IsRootFolder && f.UserId == userId);
            if (rootFolder == null) throw new Exception("Root folder not found!");

            var rootFolderDto = ConvertToDto.ConvertToFolderDto(rootFolder);
            return rootFolderDto; }
            catch { throw; }

            
        }

        public UserTrashContent GetUserTrash(int userId)
        {
            try {var trashFolders = _context.Folders.Where(f => f.UserId == userId && f.IsDeleted).ToList();
            var trashFiles = _context.Files.Where(f => f.UserId == userId && f.IsDeleted).ToList();

            var trashFoldersDto = trashFolders.Select(t => ConvertToDto.ConvertToFolderDto(t)).ToList();
            var trashFilesDto = trashFiles.Select(t => ConvertToDto.ConvertToFileDto(t)).ToList();
            
            return new UserTrashContent(trashFilesDto, trashFoldersDto); }
            catch { throw; }
            
        }

        public async Task UpdateUser(int userId, UpdateUserRequest request)
        {
            // Validate request
            if (string.IsNullOrEmpty(request.NewFullName) && string.IsNullOrEmpty(request.NewEmail))
            {
                throw new Exception("Invalid request.");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found!");

            if (!string.IsNullOrEmpty(request.NewEmail))
            {
                if (!Regex.IsMatch(request.NewEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new Exception("Invalid email format");
                else
                    user.Email = request.NewEmail;
            }

            if (!string.IsNullOrEmpty(request.NewFullName))
                user.FullName = request.NewFullName;

            await _context.SaveChangesAsync();
        }
    }
}
