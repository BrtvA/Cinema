using Cinema.DAL.Entities;

namespace Cinema.DAL.Repositories.Interfaces;

public interface IGenreRepository
{
    public Task CreateAsync(Genre genre);
    public Task<Genre?> GetAsync(int id);
    public Task<Genre?> GetByNameAsync(string name);
    public Task<IEnumerable<Genre>> ListAsync(int skipSize, int count);
    public Task<IEnumerable<Genre>> ListAsync();
    public Task<int> GetCountAsync();
    public void Update(Genre genre);
    public void Delete(Genre genre);
}
