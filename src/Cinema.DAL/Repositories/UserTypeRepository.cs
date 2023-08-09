using Cinema.DAL.Entities;
using Cinema.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DAL.Repositories;

public class UserTypeRepository : IUserTypeRepository
{
    private readonly ApplicationContext _db;

    public UserTypeRepository(ApplicationContext applicationContext)
    {
        _db = applicationContext;
    }

    public async Task<UserType?> GetAsync(int id)
    {
        return await _db.UserTypes.FirstOrDefaultAsync(ut => ut.Id == id);
    }
}
