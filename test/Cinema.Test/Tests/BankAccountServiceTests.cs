using Cinema.BLL;
using Cinema.BLL.CustomExceptions;
using Cinema.BLL.DTOs.Request;
using Cinema.BLL.Services;
using Cinema.BLL.Services.Interfaces;
using Cinema.DAL.UnitOfWorks;
using Cinema.Test.Infrastructure.Helpers;
using Cinema.Test.Infrastructure.Supports;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Cinema.Test.Tests;

[Collection("Database collection")]
public class BankAccountServiceTests
{
    private readonly IBankAccountService _bankAccountService;

    public BankAccountServiceTests(DatabaseFixture fixture)
    {
        var contextHelper = new ApplicationContextHelper();
        IUnitOfWork unitOfWork = new UnitOfWork(contextHelper.Options);
        _bankAccountService = new BankAccountService(unitOfWork);
    }

    private async Task<ServiceResult<string>> PromtBuyAsync(
        string cardNumber, string action)
    {
        await using var application = new WebApplicationFactory<Program>();
        var dto = new BankAccountReqDTO
        {
            GuidId = Guid.Parse("E3DD38B6-BB92-4283-AB6F-EF30DFE7A8BF"),
            CardNumber = cardNumber,
            MonthEnd = 1,
            YearEnd = 30,
            Cvc = 123
        };

        return await _bankAccountService.BuyAsync(
                dto, application.CreateClient(), action);
    }

    [Fact]
    public async Task BuyAsync_BankAccountReqDTOHttpClientAndString_ResultOk()
    {
        string cardNumber = "1111222233334444";
        string action = "/buy-info";
        string expected = "Ok";

        var result = await PromtBuyAsync(cardNumber, action);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task BuyAsync_BankAccountReqDTOHttpClientAndString_ResultInternalServerErrorException()
    {
        string cardNumber = "1111222233334444";
        string action = "/buy-info-none";
        string expected = "Ошибка при оплате";

        var result = await PromtBuyAsync(cardNumber, action);

        Assert.IsType<InternalServerErrorException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }

    [Theory]
    [InlineData("1111222233334445", "/buy-info")]
    [InlineData("1111222233334444", "/buy-info-to-much")]
    [InlineData("1111222233334444", "/buy-info-card-invalid")]
    public async Task BuyAsync_BankAccountReqDTOHttpClientAndString_ResultBadRequestException(
        string cardNumber, string action)
    {
        var result = await PromtBuyAsync(cardNumber, action);

        Assert.IsType<BadRequestException>(result.Exception);
    }
}
