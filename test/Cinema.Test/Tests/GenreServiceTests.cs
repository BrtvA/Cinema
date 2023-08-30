using Cinema.BLL;
using Cinema.BLL.CustomExceptions;
using Cinema.BLL.DTOs.Request;
using Cinema.BLL.Services;
using Cinema.BLL.Services.Interfaces;
using Cinema.DAL.Entities;
using Cinema.DAL.UnitOfWorks;
using Cinema.Test.Infrastructure.Helpers;

namespace Cinema.Test.Tests;

[Collection("Database collection")]
public class GenreServiceTests
{
    private readonly IGenreService _genreService;

    public GenreServiceTests()
    {
        var contextHelper = new ApplicationContextHelper();
        IUnitOfWork unitOfWork = new UnitOfWork(contextHelper.Options);
        _genreService = new GenreService(unitOfWork);
    }

    #region CreateAsync
    //..............Проверка создания жанра..............//

    [Fact]
    public async Task CreateAsync_GenreReqDTO_ResultOk()
    {
        var dto = new GenreReqDTO
        {
            Name = "Создание"
        };
        string expected = "Ok";

        var result = await _genreService.CreateAsync(dto);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task CreateAsync_GenreReqDTO_ResultBadRequestException()
    {
        var dto = new GenreReqDTO
        {
            Name = "Тестовый 1"
        };
        string expected = "Такой жанр уже существует";

        var result = await _genreService.CreateAsync(dto);

        Assert.IsType<BadRequestException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion

    #region ListAsync
    //..............Проверка получения списка жанров..............//

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

    private async Task<ServiceResult<string>> PromtUpdateAsync(int id)
    {
        var genre = new Genre
        {
            Id = id,
            Name = "Тестовый редактированный"
        };

        return await _genreService.UpdateAsync(genre);
    }

    [Fact]
    public async Task UpdateAsync_Genre_ResultOk()
    {
        int id = 5;
        string expected = "Ok";

        var result = await PromtUpdateAsync(id);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task UpdateAsync_Genre_ResultNotFoundException()
    {
        int id = 100;
        string expected = "Такого жанра не существует";

        var result = await PromtUpdateAsync(id);

        Assert.IsType<NotFoundException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion

    #region DeleteAsync
    //..............Проверка удаления жанров..............//

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

    [Fact]
    public async Task DeleteAsync_Integer_ResultBadRequestException()
    {
        int id = 1;
        string expected = "Данный жанр используется";

        var result = await _genreService.DeleteAsync(id);

        Assert.IsType<BadRequestException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion
}
