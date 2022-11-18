namespace JwtAuthServer.Models;

public class User
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string ValidationCode { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsLocked { get; set; }
}