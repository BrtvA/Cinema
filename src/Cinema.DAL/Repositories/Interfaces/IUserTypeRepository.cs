using Cinema.DAL.Entities;

namespace Cinema.DAL.Repositories.Interfaces;

public interface IUserTypeRepository
{
    public Task<UserType?> GetAsync(int id);
}
