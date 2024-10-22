using MainPortfolio.Models;

namespace MainPortfolio.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task<bool> IsEmailTakenAsync(string email);
    Task<int> SaveChangesAsync();
    Task<sbyte> IsAdmin();
}
