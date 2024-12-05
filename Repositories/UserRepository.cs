using Microsoft.EntityFrameworkCore;

using MainPortfolio.Repositories.Interfaces;
using MainPortfolio.Models;
using MainPortfolio.Data;

namespace MainPortfolio.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) { _context = context; }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<sbyte> IsAdmin()
    {
        // first user beccomes admin (Role = 1), others are customers (Role = 0)
        return (sbyte)(!(await _context.Users.AnyAsync()) ? 1 : 0);
    }
}
