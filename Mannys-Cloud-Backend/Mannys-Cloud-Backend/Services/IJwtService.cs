using Mannys_Cloud_Backend.Models;

namespace Mannys_Cloud_Backend.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
