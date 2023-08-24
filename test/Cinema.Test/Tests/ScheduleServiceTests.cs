using Cinema.BLL;
using Cinema.BLL.CustomExceptions;
using Cinema.BLL.DTOs.Request;
using Cinema.BLL.DTOs.Response;
using Cinema.BLL.Services;
using Cinema.BLL.Services.Interfaces;
using Cinema.DAL.Models.Base;
using Cinema.DAL.UnitOfWorks;
using Cinema.Test.Infrastructure.Helpers;

namespace Cinema.Test.Tests;

[Collection("Database collection")]
public class ScheduleServiceTests
{
    private readonly IScheduleService _scheduleService;

    public ScheduleServiceTests()
    {
        var contextHelper = new ApplicationContextHelper();
        IUnitOfWork unitOfWork = new UnitOfWork(contextHelper.Options);
        _scheduleService = new ScheduleService(unitOfWork);
    }

    #region CreateAsync
    //..............Проверка создания афиши..............//
    //Добавить проверку выбрасывания исключений

    private async Task<ServiceResult<string>> PromtCreateAsync(
        int movieId, int hallId, DateTime startTime)
    {
        var dto = new ScheduleReqDTO
        {
            MovieId = movieId,
            HallId = hallId,
            StartTime = startTime,
        };

        return await _scheduleService.CreateAsync(dto);
    }

    [Fact]
    public async Task CreateAsync_ScheduleReqDTO_ResultOk()
    {
        int movieId = 1;
        int hallId = 2;
        DateTime startTime = DateTime.Now.AddDays(1);
        string expected = "Ok";

        var result = await PromtCreateAsync(movieId, hallId, startTime);

        Assert.Equal(expected, result.Value);
    }

    [Theory]
    [InlineData(100, 1, 9, false)]
    [InlineData(1, 100, 9, false)]
    [InlineData(1, 1, 9, false)]
    [InlineData(1, 1, 9, true)]
    public async Task CreateAsync_ScheduleReqDTO_ResultBadRequestException(
        int movieId, int hallId, int hour, bool inPast)
    {
        var currentDate = DateTime.Now;
        var startTime = new DateTime(
                currentDate.Year, currentDate.Month, 
                inPast ? currentDate.Day - 1 : currentDate.Day,
                hour, 0, 0);

        var result = await PromtCreateAsync(movieId, hallId, startTime);

        Assert.IsType<BadRequestException>(result.Exception);
    }
    #endregion

    #region GetAsync
    //..............Проверка получения афиши..............//

    [Fact]
    public async Task GetAsync_Integer_ResultOk()
    {
        int id = 1;

        var result = await _scheduleService.GetAsync(id);

        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task GetAsync_Integer_ResultNotFoundException()
    {
        int id = 100;
        string expected = "Такой записи не существует";

        var result = await _scheduleService.GetAsync(id);

        Assert.IsType<NotFoundException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion

    #region GetPriceAsync
    //..............Проверка получения цены..............//
    //Проверка NotFoundException для movie

    private async Task<ServiceResult<decimal>> PromtGetPriceAsync(int id)
    {
        int ticketCount = 1;
        decimal discountCoefficient = 0.95m;
        decimal reductionFactorPrice = 0.9m;
        decimal multiplyingFactorPrice = 1.1m;
        int reductionBoundaryTime = 10;
        int multiplyingBoundaryTime = 16;

        return await _scheduleService.GetPriceAsync(
            id, ticketCount, discountCoefficient,
            reductionFactorPrice, multiplyingFactorPrice,
            reductionBoundaryTime, multiplyingBoundaryTime);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task GetPriceAsync_IntegersAndDecimals_ResultOk(int id)
    {
        var result = await PromtGetPriceAsync(id);

        Assert.IsType<decimal>(result.Value);
    }

    [Fact]
    public async Task GetPriceAsync_IntegersAndDecimals_ResultNotFoundException()
    {
        int id = 100;

        var result = await PromtGetPriceAsync(id);

        Assert.IsType<NotFoundException>(result.Exception);
    }
    #endregion

    #region ListInfoAsync
    //..............Проверка списка афиши..............//

    private async Task<ServiceResult<ScheduleGenreListRespDTO>> PromtListInfoAsync(
        DateTime date, int pageSize)
    {
        var dto = new HomeIndexReqDTO
        {
            Page = 1,
            GenresId = null,
            Date = date.ToString("yyyy-MM-dd"),
        };
        int dayVisibilityScope = 14;

        return await _scheduleService.ListInfoAsync(dto, pageSize, dayVisibilityScope);
    }

    [Fact]
    public async Task ListInfoAsync_HomeIndexReqDTOAndTwoIntegers_ResultOk()
    {
        var date = DateTime.Now;
        int pageSize = 2;
        int expected = 1;

        var result = await PromtListInfoAsync(date, pageSize);

        Assert.NotNull(result.Value);
        Assert.False(result.Value.NextPageExist);
        Assert.NotEmpty(result.Value.GenreList);
        Assert.NotEmpty(result.Value.ScheduleList);
        Assert.Equal(expected, result.Value.ScheduleList.Count());
    }

    [Fact]
    public async Task ListInfoAsync_HomeIndexReqDTOAndTwoIntegers_ResultNotFoundException()
    {
        int addedDay = 20;
        var date = DateTime.Now.AddDays(addedDay);
        int pageSize = 2;
        string expected = "Данных не найдено";

        var result = await PromtListInfoAsync(date, pageSize);

        Assert.IsType<NotFoundException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion

    #region ListTimeAsync
    //..............Проверка времени старта кинопоказа..............//

    private async Task<ServiceResult<IEnumerable<BaseShortModel>>> PromtListTimeAsync(DateTime date)
    {
        var dto = new HomeTimeReqDTO
        {
            MovieId = 1,
            HallId = 1,
            Date = date.ToString("yyyy-MM-dd")
        };
        int dayVisibilityScope = 14;

        return await _scheduleService.ListTimeAsync(dto, dayVisibilityScope);
    }

    [Fact]
    public async Task ListTimeAsync_HomeTimeReqDTOAndInteger_ResultOk()
    {
        DateTime date = DateTime.Now;

        var result = await PromtListTimeAsync(date);

        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(2)]
    public async Task ListTimeAsync_HomeTimeReqDTOAndInteger_ResultNotFoundException(int addedDay)
    {
        DateTime date = DateTime.Now.AddDays(addedDay);

        var result = await PromtListTimeAsync(date);

        Assert.IsType<NotFoundException>(result.Exception);
    }
    #endregion

    #region ListAsync
    //..............Проверка списка афиши..............//

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ListAsync_AdminScheduleReqDTOAndInteger_ResultOk(bool dateEmpty)
    {
        var dto = new AdminScheduleReqDTO
        {
            Page = 1,
            HallId = 1,
            GenresId = null,
            Date = dateEmpty ? "" : DateTime.Now.ToString("yyyy-MM-dd"),
        };
        int pageSize = 2;

        var result = await _scheduleService.ListAsync(dto, pageSize);

        Assert.NotNull(result.Value);
        Assert.True(result.Value.NextPageExist);
        Assert.NotEmpty(result.Value.GenreList);
        Assert.NotEmpty(result.Value.MovieList);
        Assert.NotEmpty(result.Value.HallList);
        Assert.NotEmpty(result.Value.ScheduleList);
        Assert.Equal(pageSize, result.Value.ScheduleList.Count());
    }
    #endregion

    #region UpdateAsync
    //..............Проверка списка афиши..............//
    //Добавить проверку выбрасывания исключений

    private async Task<ServiceResult<string>> PromtUpdateAsync(
        int id, int movieId, int hallId, DateTime startTime)
    {
        var dto = new ScheduleReqDTO
        {
            Id = id,
            MovieId = movieId,
            HallId = hallId,
            StartTime = startTime,
        };

        return await _scheduleService.UpdateAsync(dto);
    }

    [Fact]
    public async Task UpdateAsync_AdminScheduleReqDTOAndInteger_ResultOk()
    {
        int id = 4;
        int movieId = 1;
        int hallId = 2;
        var startTime  = DateTime.Now;
        string expected = "Ok";

        var result = await PromtUpdateAsync(
                id, movieId, hallId, startTime);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task UpdateAsync_AdminScheduleReqDTOAndInteger_ResultNotFoundException()
    {
        int id = 100;
        int movieId = 1;
        int hallId = 2;
        var startTime = DateTime.Now;
        
        string expected = "Такой записи не существует";

        var result = await PromtUpdateAsync(id, movieId, hallId, startTime);

        Assert.IsType<NotFoundException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }

    [Theory]
    [InlineData(1, 100, 2, 9)]
    [InlineData(1, 1, 100, 9)]
    [InlineData(1, 1, 1, 9)]
    public async Task UpdateAsync_AdminScheduleReqDTOAndInteger_ResultBadRequestException(
        int id, int movieId, int hallId, int hour)
    {
        var currentDate = DateTime.Now;
        var startTime = new DateTime(
                currentDate.Year, currentDate.Month, currentDate.Day,
                hour, 0, 0);

        var result = await PromtUpdateAsync(id, movieId, hallId, startTime);

        Assert.IsType<BadRequestException>(result.Exception);
    }
    #endregion

    #region DeleteAsync
    //..............Проверка удаления афиши..............//
    //Добавить проверку выбрасывания исключений

    [Fact]
    public async Task DeleteAsync_Integer_ResultOk()
    {
        int id = 5;
        string expected = "Ok";

        var result = await _scheduleService.DeleteAsync(id);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task DeleteAsync_Integer_ResultNotFoundException()
    {
        int id = 100;
        string expected = "Такой записи не существует";

        var result = await _scheduleService.DeleteAsync(id);

        Assert.IsType<NotFoundException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion
}
