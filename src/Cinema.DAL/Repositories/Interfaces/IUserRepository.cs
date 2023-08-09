using Cinema.DAL.Entities;

namespace Cinema.DAL.Repositories.Interfaces;

public interface IUserRepository
{
    public Task CreateAsync(User user);
    public Task<User?> GetByEmailAsync(string email);
}
