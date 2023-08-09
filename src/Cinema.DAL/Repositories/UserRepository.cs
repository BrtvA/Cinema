using Cinema.DAL.Entities;
using Cinema.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationContext _db;

    public UserRepository(ApplicationContext applicationContext)
    {
        _db = applicationContext;   
    }

    public async Task CreateAsync(User user)
    {
        await _db.Users.AddAsync(user);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(usr => usr.Email == email);
    }
}
