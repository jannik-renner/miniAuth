using Microsoft.IdentityModel.Tokens;
using MiniAuth.Application.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace MiniAuth.Infrastructure.Security
{
    public sealed class JwtTokenService : ITokenService
    {
        private readonly JwtOptions _options;

        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public string CreateAccessToken(Guid userId, string email, IEnumerable<string> roles)
        {
            var secret = _options.Secret;

            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException("JWT secret is not configured.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.NameId, userId.ToString()),
                new(JwtRegisteredClaimNames.Email, email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var expiresAt = GetAccessTokenExpiresAt();

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(bytes);
        }

        public DateTime GetAccessTokenExpiresAt()
        {
            return DateTime.UtcNow.AddMinutes(_options.AccessTokenLifetimeMinutes);
        }

        public DateTime GetRefreshTokenExpiresAt()
        {
            return DateTime.UtcNow.AddDays(_options.RefreshTokenLifetimeDays);
        }
    }
}
