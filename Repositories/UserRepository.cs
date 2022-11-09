using JwtAuthServerApi.Data;
using JwtAuthServerApi.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace JwtAuthServerApi.Repositories;

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
    public async Task<User?> GetUserByUsername(string username)
    {
        return await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
    }
    public async Task<User> GetUserByEmail(string email)
    {
        return await _context.Users.Where(u => u.Email == email).FirstAsync();
    }
    public async Task<User> AddUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return (await GetUserByUsername(user.Username))!;
    }
    public async Task<User> UpdateUser(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return await _context.Users.Where(u => u.Username == user.Username).FirstAsync();
    }
    public async Task<User> LockUser(User user)
    {
        user.IsLocked = true;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return await _context.Users.Where(u => u.Username == user.Username).FirstAsync();
    }
}