using JwtAuthServer.Models;
using JwtAuthServer.ViewModels;

namespace JwtAuthServer.Services;

public interface IAccountService
{
    Task<User> Register(RegisterViewModel request);
    Task<User> Login(LoginViewModel request);
    Task<User> GetAccount(string identifier);
    Task<User> VerifyAccount(string identifier, string? phoneNumber, string code, string password);
    Task SaveRefreshToken(string resultIdentifier, string refreshToken, DateTime expires);
    Task GenerateAndSendVerificationCode(string identifier, string? phoneNumber);
}