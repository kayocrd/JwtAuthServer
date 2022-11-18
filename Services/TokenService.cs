using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JwtAuthServer.Models;
using JwtAuthServer.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthServer.Services;

public class TokenService
{
    private readonly UserRepository _userRepository;
    public TokenService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Settings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Identifier),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                new Claim("IsVerified", user.IsVerified.ToString()),
                new Claim("IsLocked", user.IsLocked.ToString())
            }),
            Issuer = Settings.Issuer,
            Expires = DateTime.UtcNow.AddHours(Settings.ExpirationHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
            return  tokenHandler.WriteToken(token);
    }
    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Settings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = Settings.Issuer,
            Expires = DateTime.UtcNow.AddHours(Settings.ExpirationHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    public string GenerateRefreshToken()
    {
        var randomNumbers = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumbers);
        return Convert.ToBase64String(randomNumbers);
    }
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuer = Settings.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.Secret)),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, 
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
    public async Task SaveRefreshToken(string identifier, string refreshToken, DateTime expirationDate)
    {
        var user = await _userRepository.GetUserByIdentifier(identifier);
        if (user is null)
            throw new Exception("User not found");
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = expirationDate;
        await _userRepository.UpdateUser(user);
    }
    public async Task<bool> RevokeByIdentifier(string identifier)
    {
        var user = await _userRepository.GetUserByIdentifier(identifier);
        if (user is null)
            throw new Exception("User not found");
        user.RefreshToken = string.Empty;
        user.RefreshTokenExpiryTime = DateTime.MinValue.ToUniversalTime();
        await _userRepository.UpdateUser(user);
        return true;
    }
    public async Task<string> GetRefreshToken(string identifier)
    {
        var user = await _userRepository.GetUserByIdentifier(identifier);
        if (user is null)
            throw new Exception("User not found");
        return user.RefreshToken;
    }
}