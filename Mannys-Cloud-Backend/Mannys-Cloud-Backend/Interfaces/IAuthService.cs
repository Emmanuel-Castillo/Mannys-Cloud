using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Services;

namespace Mannys_Cloud_Backend.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthenticatedUserData> Register(RegisterUserRequest request);

        public Task<AuthenticatedUserData> Login(LoginUserRequest request);

        public Task<UserDto> CheckAuth(int userId);
    }
}
