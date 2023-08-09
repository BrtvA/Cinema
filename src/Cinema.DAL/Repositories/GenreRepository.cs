using Cinema.DAL.Entities;
using Cinema.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DAL.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly ApplicationContext _db;

    public GenreRepository(ApplicationContext applicationContext)
    {
        _db = applicationContext;
    }

    public async Task CreateAsync(Genre genre)
    {
        await _db.Genres.AddAsync(genre);
    }

    public async Task<Genre?> GetAsync(int id)
    {
        return await _db.Genres.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Genre?> GetByNameAsync(string name)
    {
        return await _db.Genres.FirstOrDefaultAsync(g => g.Name == name);
    }

    private IOrderedQueryable<Genre> GenreOrdered()
    {
        return _db.Genres.OrderBy(g => g.Id);
    }

    public async Task<IEnumerable<Genre>> ListAsync(int skipSize, int count)
    {
        return await GenreOrdered().Skip(skipSize).Take(count).ToListAsync();
    }

    public async Task<IEnumerable<Genre>> ListAsync()
    {
        return await GenreOrdered().ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _db.Genres.CountAsync();
    }

    public void Update(Genre genre)
    {
        _db.Genres.Update(genre);
    }

    public void Delete(Genre genre)
    {
        _db.Genres.Remove(genre);
    }
}
