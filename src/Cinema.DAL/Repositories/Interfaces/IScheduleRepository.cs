using Cinema.DAL.Entities;
using Cinema.DAL.Models;
using Cinema.DAL.Models.Base;

namespace Cinema.DAL.Repositories.Interfaces;

public interface IScheduleRepository
{
    public Task CreateAsync(Schedule schedule);

    public Task<Schedule?> GetAsync(int id);

    public Task<Schedule?> GetByFilterAsync(
        int hallId, DateTime startTime, DateTime endTime);

    public Task<IEnumerable<ScheduleModel>> ListAsync(
        int skipSize, int count, int[]? genresId, int hallId, DateTime startDate);

    public Task<IEnumerable<ScheduleInfoModel>> ListActualInfoAsync(
        int skipSize, int count, int[]? genresId, DateTime startDate);

    public Task<IEnumerable<BaseShortModel>> ListTimeAsync(
        int movieId, int hallId, DateTime startDate);

    public Task<int> GetCountInfoAsync(int[]? genresId, DateTime startDate);

    public Task<int> GetCountAsync(int[]? genresId, int hallId, DateTime startDate);

    public Task<bool> ExistByMovieIdAsync(int movieId);

    public void Update(Schedule schedule);

    public void Delete(Schedule schedule);
}
