using Cinema.DAL.Entities;
using Cinema.DAL.Models;
using Cinema.DAL.Models.Base;
using Cinema.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DAL.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly ApplicationContext _db;

    public MovieRepository(ApplicationContext applicationContext)
    {
        _db = applicationContext;
    }

    public async Task CreateAsync(Movie movie)
    {
        await _db.Movies.AddAsync(movie);
    }

    public async Task<Movie?> GetAsync(int id)
    {
        return await _db.Movies.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Movie?> GetByNameAsync(string name)
    {
        return await _db.Movies.FirstOrDefaultAsync(m => m.Name == name);
    }

    private IQueryable<Movie> MoviesActual()
    {
        return _db.Movies.Where(m => m.IsActual);
    }

    public async Task<MovieInfoModel?> GetActualInfoAsync(int id)
    {
        return await (from m in MoviesActual()
                      join mg in _db.MovieGenres
                        on m.Id equals mg.MovieId
                      join g in _db.Genres
                        on mg.GenreId equals g.Id
                      group new { g } by new { m.Id, m.Name, m.Description, m.ImageName, m.Duration } into mm
                      select new MovieInfoModel
                      {
                          Id = mm.Key.Id,
                          Name = mm.Key.Name,
                          Description = mm.Key.Description,
                          ImageName = mm.Key.ImageName,
                          Duration = mm.Key.Duration,
                          Genres = EF.Functions.ArrayAgg(mm.Select(x => x.g.Name)),
                      }).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<BaseShortModel>> ListActualAsync()
    {
        return await MoviesActual()
            .Select(m => new BaseShortModel
            {
                Id = m.Id,
                Name = m.Name,
            })
            .ToListAsync();
    }

    public async Task<MovieModel?> GetInfoAsync(int id)
    {
        return await (from m in _db.Movies
                      join mg in _db.MovieGenres
                        on m.Id equals mg.MovieId
                      group new { mg } by new { m.Id, m.Name, m.Description, m.ImageName, m.Duration, m.Price, m.IsActual} into mm
                      select new MovieModel
                      {
                          Id = mm.Key.Id,
                          Name = mm.Key.Name,
                          Description = mm.Key.Description,
                          ImageName = mm.Key.ImageName,
                          Duration = mm.Key.Duration,
                          Price = mm.Key.Price,
                          GenresId = EF.Functions.ArrayAgg(mm.Select(x => x.mg.GenreId)),
                          IsActual = mm.Key.IsActual,
                      }).FirstOrDefaultAsync(x => x.Id == id);
    }

    private IQueryable<Movie> MoviesFiltered(string search)
    {
        return _db.Movies
            .Where(m => search == "" || EF.Functions.Like(m.Name, $"%{search}%"));
    }

    public async Task<IEnumerable<BaseShortModel>> ListAsync(int skipSize, int count, string search)
    {
        return await MoviesFiltered(search)
            .Select(m => new BaseShortModel { 
                Id = m.Id,
                Name = m.Name 
            })
            .OrderBy(m => m.Id)
            .Skip(skipSize).Take(count)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync(string search)
    {
        return await MoviesFiltered(search).CountAsync();
    }

    public void Update(Movie movie)
    {
        _db.Movies.Update(movie);
    }

    public void Delete(Movie movie)
    {
        _db.Movies.Remove(movie);
    }
}
