using JwtAuthServerApi.Models;
using JwtAuthServerApi.Repositories;
using JwtAuthServerApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using SecureIdentity.Password;

namespace JwtAuthServerApi.Services;

public class AccountService : IAccountService
{
    private readonly UserRepository _context;

    public AccountService(UserRepository context)
    {
        _context = context;
    }
    
    public async Task<User> Register(RegisterViewModel request)
    {
        var user = new User()
        {
            FullName = request.FullName,
            Username = request.Username,
            Email = request.Email,
        };
        user.PasswordHash = PasswordHasher.Hash(request.Password);
        await _context.AddUser(user);
        return new User()
        {
            FullName = user.FullName,
            Username = user.Username,
            Email = user.Email,
        };
    }
    public async Task<User> Login(LoginViewModel request)
    {
        var user = await _context.GetUserByUsername(request.Username);
        if (user == null)
        {
            throw new Exception("Invalid username or password");
        }
        if (!PasswordHasher.Verify(user.PasswordHash, request.Password))
        {
            throw new Exception("Invalid username or password");
        }
        return new User()
        {
            FullName = user.FullName,
            Username = user.Username,
            Email = user.Email,
            IsLocked = user.IsLocked,
        };
    }
}