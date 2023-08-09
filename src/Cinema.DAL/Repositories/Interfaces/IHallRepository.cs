using Cinema.DAL.Entities;
using Cinema.DAL.Models.Base;

namespace Cinema.DAL.Repositories.Interfaces;

public interface IHallRepository
{
    public Task<Hall?> GetAsync(int id);
    public Task<IEnumerable<Hall>> ListAsync();
    public Task<IEnumerable<BaseShortModel>> ListInfoAsync();
}
