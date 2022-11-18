using JwtAuthServer.Data;
using JwtAuthServer.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace JwtAuthServer.Repositories;

public class UserRepository
{
    private readonly  AppDbContext _context;
    
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }
    public async Task<User?> GetUserByIdentifier(string identifier)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Identifier == identifier);
    }
    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    public async Task AddUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateUser(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
    public async Task<User> LockUser(User user)
    {
        user.IsLocked = true;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return await _context.Users.FirstAsync(u => u.Identifier == user.Identifier);
    }
}