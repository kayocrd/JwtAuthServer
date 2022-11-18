using JwtAuthServer.Models;
using JwtAuthServer.Repositories;
using JwtAuthServer.ViewModels;
using SecureIdentity.Password;

namespace JwtAuthServer.Services;

public class AccountService : IAccountService
{
    private readonly UserRepository _context;
    private IWhatsappService _whatsappService;
    public AccountService(UserRepository context, IWhatsappService whatsappService)
    {
        _context = context;
        _whatsappService = whatsappService;
    }

    public async Task<User> Register(RegisterViewModel request)
    {
        if (await _context.GetUserByIdentifier(request.Identifier) != null)
        {
            throw new Exception("Identifier already exists");
        }
        if (await _context.GetUserByEmail(request.Email) != null )
        {
            throw new Exception("Email already exists");
        }
        var newUser = new User()
        {
            FullName = request.FullName,
            Identifier = request.Identifier,
            Email = request.Email,
            PhoneNumber = string.Concat(55, request.PhoneNumber),
            IsVerified = false,
            IsLocked = true,
        };
        await _context.AddUser(newUser);
        
        return new User() 
        {
          FullName = newUser.FullName,
          Identifier = newUser.Identifier,
          PhoneNumber = newUser.PhoneNumber,
          Email = newUser.Email,
          IsLocked = newUser.IsLocked,
          IsVerified = newUser.IsVerified,
        };
    }
    
    // public async Task<User> Verify(VerifyViewModel request)
    // {
    //     var user = await _context.GetUser(request.Username);
    //     if (user == null)
    //     {
    //         return null;
    //     }
    //     if (user.ValidationCode == request.ValidationCode)
    //     {
    //         user.IsVerified = true;
    //         user.IsLocked = false;
    //         await _context.UpdateUser(user);
    //         return user;
    //     }
    //     return null;
    // }

    public async Task<User> Login(LoginViewModel request)
    {
        var user = await _context.GetUserByIdentifier(request.Identifier);
        if (user == null)
        {
            throw new Exception("Invalid username or password");
        }
        if (!PasswordHasher.Verify(user.PasswordHash, request.Password))
        {
            throw new Exception("Invalid username or password");
        }
        if (user.IsVerified == false)
        {
            throw new Exception("User is not verified");
        }
        if (user.IsLocked)
        {
            throw new Exception("Your account is locked");
        }
        return new User()
        {
            FullName = user.FullName,
            Identifier = user.Identifier,
            Email = user.Email,
            IsLocked = user.IsLocked,
        };
    }
    public async Task<User> GetAccount(string identifier)
    {
        var user = await _context.GetUserByIdentifier(identifier);
        if (user == null)
        {
            throw new Exception("Account not found");
        }
        return new User()
        {
            FullName = user.FullName,
            Identifier = user.Identifier,
            PhoneNumber = user.PhoneNumber,
            ValidationCode = user.ValidationCode,
            Email = user.Email,
            IsVerified = user.IsVerified,
            IsLocked = user.IsLocked,
        };
    }
    public async Task<User> VerifyAccount(string identifier, string? phoneNumber, string code, string password)
    {
        var user = await _context.GetUserByIdentifier(identifier);
        if (user == null)
            throw new Exception("Invalid username");
        if (user.IsVerified == true)
            throw new Exception("User already verified");
        if (code != user.ValidationCode) throw new Exception("Invalid code");
        user.PasswordHash = PasswordHasher.Hash(password);
        if (phoneNumber != null) user.PhoneNumber = phoneNumber;
        user.IsVerified = true;
        user.ValidationCode = string.Empty;
        user.IsLocked = false;
        await _context.UpdateUser(user);
        return new User()
        {
            FullName = user.FullName,
            Identifier = user.Identifier,
            Email = user.Email,
            IsVerified = user.IsVerified,
            IsLocked = user.IsLocked,
        };
    }
    public async Task SaveRefreshToken(string identifier, string refreshToken, DateTime expires)
    {
        var user = await _context.GetUserByIdentifier(identifier);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = expires;
        await _context.UpdateUser(user);
    }
    public async Task GenerateAndSendVerificationCode(string identifier, string? phoneNumber)
    {
        try
        {
            var user = await _context.GetUserByIdentifier(identifier);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (phoneNumber != null) user.PhoneNumber = phoneNumber;
            if (user.IsVerified == true) 
            {
                throw new Exception("User already verified");
            }
            var code = new Random().Next(100000, 999999).ToString();
            user.ValidationCode = code;
            await _context.UpdateUser(user);
            await _whatsappService.SendWhatsappMessage(user.PhoneNumber, $"Olá, seu código de verificação é: *{code}*");
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}