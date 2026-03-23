using System.Security.Claims;
using OleAI.Backend.Core.Security.Jwt;

namespace OleAI.Backend.Core.Helpers
{
    public interface ITokenHelper
    {
        AccessToken CreateToken(IEnumerable<Claim> claims);
        RefreshToken CreateRefreshToken();
    }
}
