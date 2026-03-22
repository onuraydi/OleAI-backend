using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OleAI.Backend.Domain.Helpers;

namespace OleAI.Backend.Core.Security.Jwt
{
    public class JwtHelper : ITokenHelper
    {

        private readonly TokenOptions _tokenOptions;

        public JwtHelper(IConfiguration configuration)
        {
            _tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>()
                ?? throw new ArgumentNullException("TokenOptions configuration not found.");
        }

        public RefreshToken CreateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expiration = DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenExpiration)
            };
        }

        public AccessToken CreateToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecurityKey));
            var crededentals = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
            var expirationDate = DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenExpiration);

            var jwtToken = new JwtSecurityToken(
                    issuer: _tokenOptions.Issuer,
                    audience: _tokenOptions.Audience,
                    claims: claims,
                    expires: expirationDate,
                    signingCredentials: crededentals
                );

            var tokenHandler = new JwtSecurityTokenHandler();

            return new AccessToken
            {
                Token = tokenHandler.WriteToken(jwtToken),
                Expriration = expirationDate
            };
        }
    }
}
