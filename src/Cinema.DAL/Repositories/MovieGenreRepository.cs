using Cinema.DAL.Entities;
using Cinema.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DAL.Repositories;

public class MovieGenreRepository : IMovieGenreRepository
{
    private readonly ApplicationContext _db;

    public MovieGenreRepository(ApplicationContext applicationContext)
    {
        _db = applicationContext;
    }

    public async Task CreateAsync(IEnumerable<MovieGenre> movieGenreList)
    {
        await _db.MovieGenres.AddRangeAsync(movieGenreList);
    }

    public async Task<IEnumerable<MovieGenre>> ListAsync(int movieId)
    {
        return await _db.MovieGenres.Where(mg => mg.MovieId == movieId).ToListAsync();
    }

    public async Task<bool> ExistByGenreIdAsync(int genreId)
    {
        return await _db.MovieGenres.Where(mg => mg.GenreId == genreId).AnyAsync();
    }

    public void Delete(IEnumerable<MovieGenre> movieGenreList)
    {
        _db.MovieGenres.RemoveRange(movieGenreList);
    }
}
