using Cinema.BLL;
using Cinema.BLL.CustomExceptions;
using Cinema.BLL.DTOs.Request;
using Cinema.BLL.DTOs.Response;
using Cinema.BLL.Services;
using Cinema.BLL.Services.Interfaces;
using Cinema.DAL.UnitOfWorks;
using Cinema.Test.Infrastructure.Helpers;
using Cinema.Test.Infrastructure.Supports;
using Microsoft.AspNetCore.Http;

namespace Cinema.Test.Tests;

[Collection("Database collection")]
public class MovieServiceTests
{
    private readonly IMovieService _movieService;
    private readonly string? _fullPath = null;
    private readonly string? _pathToSave = null;

    public MovieServiceTests(DatabaseFixture fixture)
    {
        var contextHelper = new ApplicationContextHelper();
        if (fixture.AppDir is not null)
        {
            _fullPath = Path.Combine(fixture.AppDir, "Resourses/test_image.jpg");
            _pathToSave = Path.Combine(fixture.AppDir, "ImageDb");
        }

        IUnitOfWork unitOfWork = new UnitOfWork(contextHelper.Options);
        _movieService = new MovieService(unitOfWork);
    }

    #region CreateAsync
    //..............Проверка создания фильма..............//

    private async Task<ServiceResult<string>> PromtCreateAsync(string name, bool fileExist)
    {
        Assert.NotNull(_fullPath);
        Assert.NotNull(_pathToSave);

        IFormFile? file = null;
        if (fileExist)
        {
            byte[] fileBytes = File.ReadAllBytes(_fullPath);
            file = new FormFile(
                new MemoryStream(fileBytes),
                0, fileBytes.Length,
                "Data", "test_image.jpg");
        }

        var dto = new MovieReqDTO
        {
            Name = name,
            Description = "Тестовое описание",
            Image = file,
            GenresId = new[] { 1, 2 },
            Duration = 100,
            Price = 300m,
            IsActual = true,
        };

        return await _movieService.CreateAsync(dto, _pathToSave);
    }

    [Fact]
    public async Task CreateAsync_MovieReqDTOAndString_ResultOk()
    {
        string movieName = "Тестовый фильм";
        bool fileExist = true;
        string expected = "Ok";

        var result = await PromtCreateAsync(movieName, fileExist);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task CreateAsync_MovieReqDTOAndString_ResultBadRequestException()
    {
        string movieName = "Тестовый фильм 1";
        bool fileExist = true;
        string expected = "Такой фильм уже существует";

        var result = await PromtCreateAsync(movieName, fileExist);

        Assert.IsType<BadRequestException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }

    [Fact]
    public async Task CreateAsync_MovieReqDTOAndString_ResultError()
    {
        string movieName = "Тестовый фильм с ошибкой";
        bool fileExist = false;
        string expected = "Неизвестная ошибка";

        var result = await PromtCreateAsync(movieName, fileExist);

        Assert.Equal(expected, result.Value);
    }
    #endregion

    #region GetAsync
    //..............Проверка получения фильма..............//

    [Fact]
    public async Task GetAsync_Integer_ResultOk()
    {
        int id = 1;

        var result = await _movieService.GetAsync(id);

        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task GetAsync_Integer_ResultNotFoundException()
    {
        int id = 100;
        string expected = "Такого фильма не существует";

        var result = await _movieService.GetAsync(id);

        Assert.IsType<NotFoundException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion

    #region GetDetailsAndHallsAsync
    //..............Проверка получения информации о фильме и списка залов..............//

    private async Task<ServiceResult<MovieHallListRespDTO>> PromtGetDetailsAndHallsAsync(
        DateTime date, int movieId)
    {
        HomeInfoReqDTO infoDTO = new HomeInfoReqDTO
        {
            Date = date.ToString("yyyy-MM-dd"),
            MovieId = movieId,
        };
        int dayVisibilityScope = 14;

        return await _movieService.GetDetailsAndHallsAsync(infoDTO, dayVisibilityScope);
    }

    [Fact]
    public async Task GetDetailsAndHallsAsync_HomeInfoReqDTOAndInteger_ResultOk()
    {
        var date = DateTime.Now;
        int movieId = 1;

        var result = await PromtGetDetailsAndHallsAsync(date, movieId);

        Assert.NotNull(result.Value);
    }

    [Theory]
    [InlineData(20, 1)]
    [InlineData(0, 100)]
    public async Task GetDetailsAndHallsAsync_HomeInfoReqDTOAndInteger_ResultNotFoundException(int addedDay, int movieId)
    {
        var date = DateTime.Now.AddDays(addedDay);
        string expected = "Данных не найдено";

        var result = await PromtGetDetailsAndHallsAsync(date, movieId);

        Assert.IsType<NotFoundException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion

    #region ListAsync
    //..............Проверка получения списка фильмов и жанров..............//

    [Fact]
    public async Task ListAsync_AdminMovieReqDTOAndInteger_ResultOk()
    {
        var dto = new AdminMovieReqDTO
        {
            Page = 1,
            Search = ""
        };
        int pageSize = 2;

        var result = await _movieService.ListAsync(dto, pageSize);

        Assert.NotNull(result.Value);
        Assert.True(result.Value.NextPageExist);
        Assert.NotEmpty(result.Value.GenreList);
        Assert.NotEmpty(result.Value.MovieList);
        Assert.Equal(pageSize, result.Value.MovieList.Count());
    }
    #endregion

    #region UpdateAsync
    //..............Проверка обновления фильмов..............//

    private async Task<ServiceResult<string>> PromtUpdateAsync(int id, bool fileExist)
    {
        Assert.NotNull(_fullPath);
        Assert.NotNull(_pathToSave);

        IFormFile? file = null;
        if (fileExist)
        {
            byte[] fileBytes = File.ReadAllBytes(_fullPath);
            file = new FormFile(
                new MemoryStream(fileBytes),
                0, fileBytes.Length,
                "Data", "test_image.jpg");
        }

        var dto = new MovieReqDTO
        {
            Id = id,
            Name = "Тестовый фильм 2",
            Description = "Тестовое описание",
            Image = file,
            GenresId = new[] { 1, 2 },
            Duration = 100,
            Price = 300m,
            IsActual = true,
        };

        return await _movieService.UpdateAsync(dto, _pathToSave);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateAsync_MovieReqDTOAndString_ResultOk(bool fileExist)
    {
        int id = 2;
        string expected = "Ok";

        var result = await PromtUpdateAsync(id, fileExist);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task UpdateAsync_MovieReqDTOAndString_ResultNotFoundException()
    {
        int id = 100;
        bool fileExist = true;
        string expected = "Такого фильма не существует";

        var result = await PromtUpdateAsync(id, fileExist);

        Assert.IsType<NotFoundException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion

    #region DeleteAsync
    //..............Проверка удаления фильмов..............//

    [Fact]
    public async Task DeleteAsync_IntegerAndString_ResultOk()
    {
        Assert.NotNull(_pathToSave);

        int id = 4;
        string expected = "Ok";

        var result = await _movieService.DeleteAsync(id, _pathToSave);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task DeleteAsync_IntegerAndString_ResultNotFoundException()
    {
        Assert.NotNull(_pathToSave);

        int id = 100;
        string expected = "Такого фильма не существует";

        var result = await _movieService.DeleteAsync(id, _pathToSave);

        Assert.IsType<NotFoundException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }

    [Fact]
    public async Task DeleteAsync_IntegerAndString_ResultBadRequestException()
    {
        Assert.NotNull(_pathToSave);

        int id = 1;
        string expected = "Данный фильм используется";

        var result = await _movieService.DeleteAsync(id, _pathToSave);

        Assert.IsType<BadRequestException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion
}
