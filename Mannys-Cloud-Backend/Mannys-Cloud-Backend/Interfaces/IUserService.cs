using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Services;

namespace Mannys_Cloud_Backend.Interfaces
{
    public interface IUserService
    {
        public Task<UserData> GetUser(int userId);

        public Task UpdateUser(int userId, UpdateUserRequest request);

        public Task DeleteUser(int userId);

        public Task<FolderDto> GetUserRootFolder(int userId);

        public UserTrashContent GetUserTrash(int userId);
    }
}
