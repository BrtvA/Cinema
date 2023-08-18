using Cinema.BLL.CustomExceptions;
using Cinema.BLL.DTOs.Request;
using Cinema.BLL.Services;
using Cinema.BLL.Services.Interfaces;
using Cinema.DAL.UnitOfWorks;
using Cinema.Test.Infrastructure.Helpers;
using Cinema.Test.Infrastructure.Supports;

namespace Cinema.Test.Tests;

[Collection("Database collection")]
public class UserServiceTests
{
    private readonly IUserService _userService;

    public UserServiceTests(DatabaseFixture fixture)
    {
        var contextHelper = new ApplicationContextHelper();
        IUnitOfWork unitOfWork = new UnitOfWork(contextHelper.Options);
        _userService = new UserService(unitOfWork);
    }

    #region LoginAsync
    //..............Проверка логина..............//
    //Добавить проверку выбрасывания исключений
    //Добавить проверку NotFoundException для userType

    [Fact]
    public async Task LoginAsync_LoginReqDTO_ResultOk()
    {
        var dto = new LoginReqDTO
        {
            Email = "cinema@yandex.ru",
            Password = "1234"
        };

        var result = await _userService.LoginAsync(dto);

        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task LoginAsync_LoginReqDTO_ResultNotFoundException()
    {
        var dto = new LoginReqDTO
        {
            Email = "login@yandex.ru",
            Password = "1234"
        };
        string expected = "Пользователь не найден";

        var result = await _userService.LoginAsync(dto);

        Assert.IsType<NotFoundException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }

    [Fact]
    public async Task LoginAsync_LoginReqDTO_ResultBadRequestException()
    {
        var dto = new LoginReqDTO
        {
            Email = "cinema@yandex.ru",
            Password = "5678"
        };
        string expected = "Неверный пароль";

        var result = await _userService.LoginAsync(dto);

        Assert.IsType<BadRequestException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion

    #region RegisterAsync
    //..............Проверка регистрации..............//
    //Добавить проверку выбрасывания исключений

    [Fact]
    public async Task RegisterAsync_RegisterReqDTO_ResultOk()
    {
        var dto = new RegisterReqDTO
        {
            Email = "test@yandex.ru",
            Password = "1234",
            Name = "Test"
        };

        var result = await _userService.RegisterAsync(dto);

        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task RegisterAsync_RegisterReqDTO_ResultBadRequestException()
    {
        var dto = new RegisterReqDTO
        {
            Email = "cinema@yandex.ru",
            Password = "5678",
            Name = "Test"
        };
        string expected = "Такой пользователь уже существует";

        var result = await _userService.RegisterAsync(dto);

        Assert.IsType<BadRequestException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion
}
