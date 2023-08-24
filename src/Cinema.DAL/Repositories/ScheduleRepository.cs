using Cinema.DAL.Entities;
using Cinema.DAL.Models;
using Cinema.DAL.Models.Base;
using Cinema.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DAL.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    const int MAX_YEAR = 9999;
    const int SECONDS_IN_DAY = 86400;

    private readonly ApplicationContext _db;

    public ScheduleRepository(ApplicationContext applicationContext)
    {
        _db = applicationContext;
    }

    public async Task CreateAsync(Schedule schedule)
    {
        await _db.Schedules.AddAsync(schedule);
    }

    public async Task<Schedule?> GetAsync(int id)
    {
        return await _db.Schedules.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Schedule?> GetByFilterAsync(int hallId, DateTime startTime, DateTime endTime)
    {
        return await _db.Schedules
            .Where(s => s.StartTime >= startTime && s.StartTime < endTime
                     && s.HallId == hallId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<BaseShortModel>> ListTimeAsync(int movieId, int hallId, DateTime startDate)
    {
        return await _db.Schedules
            .Where(s => s.StartTime >= startDate && s.StartTime < startDate.AddSeconds(SECONDS_IN_DAY)
                     && s.MovieId == movieId && s.HallId == hallId)
            .OrderBy(s => s.StartTime)
            .Select(s => new BaseShortModel
            {
                Id = s.Id,
                Name = s.StartTime.ToString("HH:mm"),
            })
            .ToListAsync();
    }

    private IQueryable<ScheduleInfoModel> SchedulesInfo(int[]? genresId, DateTime startDate)
    {
        return (from s in _db.Schedules
                join m in _db.Movies
                  on s.MovieId equals m.Id
                join mg in _db.MovieGenres
                  on m.Id equals mg.MovieId
                join g in _db.Genres
                  on mg.GenreId equals g.Id
                where m.IsActual == true && (genresId == null || genresId.Contains(g.Id))
                && s.StartTime >= startDate && s.StartTime < startDate.AddSeconds(SECONDS_IN_DAY)
                group new { g } by new { m.Id, m.Name, m.ImageName } into sch
                select new ScheduleInfoModel
                {
                    Id = sch.Key.Id,
                    Name = sch.Key.Name,
                    ImageName = sch.Key.ImageName,
                    Genres = EF.Functions.ArrayAgg(sch.Select(x => x.g.Name).Distinct()),
                });
    }

    public async Task<IEnumerable<ScheduleInfoModel>> ListActualInfoAsync(int skipSize, int count, int[]? genresId, DateTime startDate)
    {
        return await SchedulesInfo(genresId, startDate)
            .OrderBy(s => s.Id)
            .Skip(skipSize).Take(count)
            .ToListAsync();
    }

    public async Task<int> GetCountInfoAsync(int[]? genresId, DateTime startDate)
    {
        return await SchedulesInfo(genresId, startDate).CountAsync();
    }

    private IQueryable<ScheduleModel> SchedulesData(int[]? genresId, int hallId, DateTime startDate)
    {
        return (from s in _db.Schedules
                join m in _db.Movies
                  on s.MovieId equals m.Id
                join h in _db.Halls
                  on s.HallId equals h.Id
                join mg in _db.MovieGenres
                  on m.Id equals mg.MovieId
                join g in _db.Genres
                  on mg.GenreId equals g.Id
                where m.IsActual == true && (genresId == null || genresId.Contains(g.Id)) 
                && (hallId == 0 || s.HallId == hallId)
                && (startDate.Year == MAX_YEAR
                    || (s.StartTime >= startDate && s.StartTime < startDate.AddSeconds(SECONDS_IN_DAY)))
                group new { g } by new { s.Id, m.Name, m.ImageName, HallName = h.Name, s.StartTime } into sch
                select new ScheduleModel
                {
                    Id = sch.Key.Id,
                    Name = sch.Key.Name,
                    ImageName = sch.Key.ImageName,
                    Genres = EF.Functions.ArrayAgg(sch.Select(x => x.g.Name)),
                    HallName = sch.Key.HallName,
                    StartTime = sch.Key.StartTime,
                });
    }

    public async Task<IEnumerable<ScheduleModel>> ListAsync(int skipSize, int count, int[]? genresId, int hallId, DateTime startDate)
    {
        return await SchedulesData(genresId, hallId, startDate)
            .OrderBy(s => s.StartTime)
            .Skip(skipSize).Take(count)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync(int[]? genresId, int hallId, DateTime startDate)
    {
        return await SchedulesData(genresId, hallId, startDate).CountAsync();
    }

    public async Task<bool> ExistByMovieIdAsync(int movieId)
    {
        return await _db.Schedules.Where(s => s.MovieId == movieId).AnyAsync();
    }

    public void Update(Schedule schedule)
    {
        _db.Schedules.Update(schedule);
    }

    public void Delete(Schedule schedule)
    {
        _db.Schedules.Remove(schedule);
    }
}
