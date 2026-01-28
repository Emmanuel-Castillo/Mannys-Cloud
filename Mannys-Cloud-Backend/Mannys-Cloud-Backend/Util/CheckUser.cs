using Mannys_Cloud_Backend.Models;
using System.Security.Claims;

namespace Mannys_Cloud_Backend.Util
{
    public static class CheckUser
    {
        public static int GrabParsedUserId(ClaimsPrincipal user)
        {
            if (user == null) throw new Exception("User is null");
            var authUserId = (user.FindFirst(ClaimTypes.NameIdentifier)?.Value) ?? throw new Exception("Authorized user id required.");
            var isIdNumeric = int.TryParse(authUserId, out int parsedId);
            if (!isIdNumeric) throw new Exception("User id cannot be parsed to an int.");
            return parsedId;
        }
        public static int UserIdMatchesRequestedId(ClaimsPrincipal user, int requestedId)
        {
            try { 
                var parsedId = GrabParsedUserId(user);
                if (parsedId != requestedId) throw new Exception("Mismatch between auth user id and requested id.");
                return parsedId;
            }
            catch {
                throw;
            }
            
        }
    }
}
