using Cinema.DAL.Entities;

namespace Cinema.DAL.Repositories.Interfaces;

public interface IMovieGenreRepository
{
    public Task CreateAsync(IEnumerable<MovieGenre> movieGenreList);
    public Task<IEnumerable<MovieGenre>> ListAsync(int movieId);
    public void Delete(IEnumerable<MovieGenre> movieGenreList);
}
