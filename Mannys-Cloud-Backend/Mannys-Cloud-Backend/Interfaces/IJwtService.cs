using Mannys_Cloud_Backend.Models;

namespace Mannys_Cloud_Backend.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
