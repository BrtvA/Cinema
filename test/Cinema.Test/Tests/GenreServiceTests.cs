using Cinema.BLL;
using Cinema.BLL.CustomExceptions;
using Cinema.BLL.DTOs.Request;
using Cinema.BLL.Services;
using Cinema.BLL.Services.Interfaces;
using Cinema.DAL.Entities;
using Cinema.DAL.UnitOfWorks;
using Cinema.Test.Infrastructure.Helpers;
using Cinema.Test.Infrastructure.Supports;

namespace Cinema.Test.Tests;

[Collection("Database collection")]
public class GenreServiceTests
{
    private readonly IGenreService _genreService;

    public GenreServiceTests(DatabaseFixture fixture)
    {
        var contextHelper = new ApplicationContextHelper();
        IUnitOfWork unitOfWork = new UnitOfWork(contextHelper.Options);
        _genreService = new GenreService(unitOfWork);
    }

    #region CreateAsync
    //..............Проверка создания жанра..............//
    //Добавить проверку выбрасывания исключений

    private async Task<ServiceResult<string>> PromtCreateAsync(string name)
    {
        var dto = new GenreReqDTO
        {
            Name = name
        };

        return await _genreService.CreateAsync(dto);
    }

    [Fact]
    public async Task CreateAsync_GenreReqDTO_ResultOk()
    {
        string genreName = "Создание";
        string expected = "Ok";

        var result = await PromtCreateAsync(genreName);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task CreateAsync_GenreReqDTO_ResultBadRequestException()
    {
        string genreName = "Тестовый 1";
        string expected = "Такой жанр уже существует";

        var result = await PromtCreateAsync(genreName);

        Assert.IsType<BadRequestException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion

    #region ListAsync
    //..............Проверка получения списка жанров..............//
    //Добавить проверку NotFoundException

    [Fact]
    public async Task ListAsync_TwoInteger_ResultOk()
    {
        int page = 1;
        int pageSize = 2;

        var result = await _genreService.ListAsync(page, pageSize);

        Assert.NotNull(result.Value);
        Assert.True(result.Value.NextPageExist);
        Assert.NotEmpty(result.Value.GenreList);
        Assert.Equal(pageSize, result.Value.GenreList.Count());
    }
    #endregion

    #region UpdateAsync
    //..............Проверка обновления жанров..............//
    //Добавить проверку выбрасывания исключений

    private async Task<ServiceResult<string>> PromtUpdateAsync(int id, string name)
    {
        var genre = new Genre
        {
            Id = id,
            Name = name
        };

        return await _genreService.UpdateAsync(genre);
    }

    [Fact]
    public async Task UpdateAsync_Genre_ResultOk()
    {
        int id = 5;
        string genreName = "Тестовый редактированный";
        string expected = "Ok";

        var result = await PromtUpdateAsync(id, genreName);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task UpdateAsync_Genre_ResultNotFoundException()
    {
        int id = 100;
        string genreName = "Тестовый редактированный";
        string expected = "Такого жанра не существует";

        var result = await PromtUpdateAsync(id, genreName);

        Assert.IsType<NotFoundException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion

    #region DeleteAsync
    //..............Проверка удаления жанров..............//
    //Добавить проверку выбрасывания исключений

    [Fact]
    public async Task DeleteAsync_Integer_ResultOk()
    {
        int id = 6;
        string expected = "Ok";

        var result = await _genreService.DeleteAsync(id);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task DeleteAsync_Integer_ResultNotFoundException()
    {
        int id = 100;
        string expected = "Такого жанра не существует";

        var result = await _genreService.DeleteAsync(id);

        Assert.IsType<NotFoundException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion
}
