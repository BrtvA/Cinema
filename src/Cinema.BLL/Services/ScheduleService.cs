using Cinema.BLL.CustomExceptions;
using Cinema.BLL.DTOs.Request;
using Cinema.BLL.DTOs.Response;
using Cinema.BLL.Services.Additional;
using Cinema.BLL.Services.Interfaces;
using Cinema.DAL.Entities;
using Cinema.DAL.Models.Base;
using Cinema.DAL.Repositories.Interfaces;
using Cinema.DAL.UnitOfWorks;

namespace Cinema.BLL.Services;

public class ScheduleService : IScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenreRepository _genreRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IHallRepository _hallRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public ScheduleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _genreRepository = unitOfWork.GenreRepository;
        _movieRepository = unitOfWork.MovieRepository;
        _hallRepository = unitOfWork.HallRepository;
        _scheduleRepository = unitOfWork.ScheduleRepository;
    }

    private async Task<object> CheckDTOAsync(ScheduleReqDTO scheduleDTO, DateTime? startTime = null)
    {
        var movie = await _movieRepository.GetAsync(scheduleDTO.MovieId);
        if (movie is null)
        {
            return new BadRequestException("Такого фильма не существует");
        }

        var hall = await _hallRepository.GetAsync(scheduleDTO.HallId);
        if (hall is null)
        {
            return new BadRequestException("Такого зала не существует");
        }

        var schedules = await _scheduleRepository.GetByFilterAsync(
                        scheduleDTO.HallId,
                        scheduleDTO.StartTime,
                        scheduleDTO.StartTime.AddMinutes(movie.Duration));
        if (schedules is not null && scheduleDTO.StartTime != startTime)
        {
            return new BadRequestException("В данное время уже есть киносеанс");
        }

        return "Ok";
    }

    public async Task<ServiceResult<string>> CreateAsync(ScheduleReqDTO scheduleDTO)
    {
        if (scheduleDTO.StartTime.Date < DateTime.Now.Date)
        {
            return new ServiceResult<string>(
                new BadRequestException("Нельзя создать сеанс в прошлом"));
        }

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            ServiceResult<string> result;

            var checkResult = await CheckDTOAsync(scheduleDTO);
            if (checkResult is BadRequestException ex)
            {
                result = new ServiceResult<string>(ex);
            }
            else
            {
                await _scheduleRepository.CreateAsync(new Schedule
                {
                    MovieId = scheduleDTO.MovieId,
                    HallId = scheduleDTO.HallId,
                    StartTime = scheduleDTO.StartTime,
                });
                await _unitOfWork.SaveAsync();

                result = new ServiceResult<string>("Ok");
            }

            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ServiceResult<Schedule>> GetAsync(int id)
    {
        var schedule = await _scheduleRepository.GetAsync(id);
        if (schedule is not null)
        {
            return new ServiceResult<Schedule>(schedule);
        }
        else{
            return new ServiceResult<Schedule>(
                new NotFoundException("Такой записи не существует"));
        }
    }

    public async Task<ServiceResult<decimal>> GetPriceAsync(
        int id, int ticketCount, decimal discountCoefficient,
        decimal reductionFactorPrice, decimal multiplyingFactorPrice,
        int reductionBoundaryTime, int multiplyingBoundaryTime)
    {
        var schedule = await _scheduleRepository.GetAsync(id);
        if (schedule is null)
        {
            return new ServiceResult<decimal>(
                new NotFoundException("Данные о цене могут быть неактуальны"));
        }

        var movie = await _movieRepository.GetAsync(schedule.MovieId);
        if (movie is not null)
        {
            decimal currentPrice = schedule.StartTime.Hour switch
            {
                var value when value < reductionBoundaryTime => movie.Price * reductionFactorPrice,
                var value when value < multiplyingBoundaryTime => movie.Price,
                _ => movie.Price * multiplyingFactorPrice
            };
            return new ServiceResult<decimal>(Convert.ToInt32(currentPrice * ticketCount * discountCoefficient));
        }
        else
        {
            return new ServiceResult<decimal>(
                new NotFoundException("Данные о цене могут быть неактуальны"));
        }
    }

    public async Task<ServiceResult<ScheduleGenreListRespDTO>> ListInfoAsync(
        HomeIndexReqDTO indexDTO, int pageSize, int dayShift)
    {
        DateTime startDate;
        var timeResult = TimeService.CheckStartTime(indexDTO.Date, dayShift);
        if (timeResult is NotFoundException ex)
        {
            return new ServiceResult<ScheduleGenreListRespDTO>(ex);
        }
        else
        {
            startDate = (DateTime)timeResult;
            indexDTO.Date = startDate.ToString("yyyy-MM-dd");
        }

        var genres = await _genreRepository.ListAsync();
        var schedules = await _scheduleRepository.ListActualInfoAsync(
                pageSize * (indexDTO.Page - 1), 
                pageSize, indexDTO.GenresId, startDate);

        int count = await _scheduleRepository.GetCountInfoAsync(indexDTO.GenresId, startDate);
        bool nextPageExist = (pageSize * indexDTO.Page) < count;

        return new ServiceResult<ScheduleGenreListRespDTO>(
            new ScheduleGenreListRespDTO(schedules, genres, nextPageExist));
    }

    public async Task<ServiceResult<ScheduleHallMovieGenreListRespDTO>> ListAsync(
        AdminScheduleReqDTO scheduleDTO, int pageSize)
    {
        var genres = await _genreRepository.ListAsync();
        var movies = await _movieRepository.ListActualAsync();
        var halls = await _hallRepository.ListInfoAsync();

        var startDate = TimeService.CheckDate(scheduleDTO.Date);
        scheduleDTO.Date = startDate.ToString("yyyy-MM-dd");

        var schedules = await _scheduleRepository.ListAsync(
                pageSize * (scheduleDTO.Page - 1), pageSize,
                scheduleDTO.GenresId, scheduleDTO.HallId, startDate);
        int count = await _scheduleRepository.GetCountAsync(
                scheduleDTO.GenresId, scheduleDTO.HallId, startDate);

        bool nextPageExist = (pageSize * scheduleDTO.Page) < count;

        return new ServiceResult<ScheduleHallMovieGenreListRespDTO>(
            new ScheduleHallMovieGenreListRespDTO(
                schedules, halls, movies, genres, nextPageExist));
    }

    public async Task<ServiceResult<IEnumerable<BaseShortModel>>> ListTimeAsync(
        HomeTimeReqDTO timeDTO, int dayShift)
    {
        DateTime startDate;
        var timeResult = TimeService.CheckStartTime(timeDTO.Date, dayShift);
        if (timeResult is NotFoundException ex)
        {
            return new ServiceResult<IEnumerable<BaseShortModel>>(ex);
        }
        else
        {
            startDate = (DateTime)timeResult;
        }

        var times = await _scheduleRepository.ListTimeAsync(
                timeDTO.MovieId, timeDTO.HallId, startDate);
        if (times.Any())
        {
            return new ServiceResult<IEnumerable<BaseShortModel>>(times);
        }

        return new ServiceResult<IEnumerable<BaseShortModel>>(
            new NotFoundException("Кинопоказы в данном зале отсутствуют"));
    }

    public async Task<ServiceResult<string>> UpdateAsync(ScheduleReqDTO scheduleDTO)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            ServiceResult<string> result;

            var schedule = await _scheduleRepository.GetAsync(scheduleDTO.Id);
            if (schedule is null)
            {
                result = new ServiceResult<string>(
                    new NotFoundException("Такой записи не существует"));
            }
            else
            {
                var checkResult = await CheckDTOAsync(scheduleDTO, schedule.StartTime);
                if (checkResult is BadRequestException ex)
                {
                    result = new ServiceResult<string>(ex);
                }
                else
                {
                    schedule.MovieId = scheduleDTO.MovieId;
                    schedule.HallId = scheduleDTO.HallId;
                    schedule.StartTime = scheduleDTO.StartTime;

                    _scheduleRepository.Update(schedule);
                    await _unitOfWork.SaveAsync();

                    result = new ServiceResult<string>("Ok");
                }
            }
            
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ServiceResult<string>> DeleteAsync(int id)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            ServiceResult<string> result;
            var schedule = await _scheduleRepository.GetAsync(id);
            if (schedule is null)
            {
                result = new ServiceResult<string>(
                    new NotFoundException("Такой записи не существует"));
            }
            else
            {
                _scheduleRepository.Delete(schedule);
                await _unitOfWork.SaveAsync();

                result = new ServiceResult<string>("Ok");
            }

            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
