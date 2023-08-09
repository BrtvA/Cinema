using Cinema.DAL.Entities;
using Cinema.DAL.Models;
using Cinema.DAL.Models.Base;

namespace Cinema.DAL.Repositories.Interfaces;

public interface IMovieRepository
{
    public Task CreateAsync(Movie movie);
    public void Update(Movie movie);
    public void Delete(Movie movie);
    public Task<Movie?> GetAsync(int id);
    public Task<MovieInfoModel?> GetActualInfoAsync(int id);
    public Task<MovieModel?> GetInfoAsync(int id);
    public Task<Movie?> GetByNameAsync(string name);
    public Task<IEnumerable<BaseShortModel>> ListActualAsync();
    public Task<IEnumerable<BaseShortModel>> ListAsync(int skipSize, int count, string search);
    public Task<int> GetCountAsync(string search);
}
