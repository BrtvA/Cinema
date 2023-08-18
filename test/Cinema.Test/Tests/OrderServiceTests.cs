using Cinema.BLL;
using Cinema.BLL.CustomExceptions;
using Cinema.BLL.DTOs.Request;
using Cinema.BLL.Services;
using Cinema.BLL.Services.Interfaces;
using Cinema.DAL.Entities;
using Cinema.DAL.Models.Base;
using Cinema.DAL.UnitOfWorks;
using Cinema.Test.Infrastructure.Helpers;
using Cinema.Test.Infrastructure.Supports;

namespace Cinema.Test.Tests;

[Collection("Database collection")]
public class OrderServiceTests
{
    private readonly IOrderService _orderService;

    public OrderServiceTests(DatabaseFixture fixture)
    {
        var contextHelper = new ApplicationContextHelper();
        IUnitOfWork unitOfWork = new UnitOfWork(contextHelper.Options);
        _orderService = new OrderService(
            unitOfWork, new ScheduleService(unitOfWork));
    }

    #region CreateAsync
    //..............Проверка списка афиши..............//
    //Добавить проверку выбрасывания исключений
    //Добавить проверку NotFoundException("Такого зала не существует"))

    private async Task<ServiceResult<string>> PromtCreateAsync(
        int scheduleId, int row, int column, string? email)
    {
        var dto = new OrderReqDTO
        {
            ScheduleId = scheduleId,
            Positions = new[]
            {
                new PositionModel { Row = row, Column = column },
            }
        };

        return await _orderService.CreateAsync(dto, email);
    }

    [Theory]
    [InlineData("cinema@yandex.ru", 1)]
    [InlineData(null, 2)]
    public async Task CreateAsync_OrderReqDTOAndString_ResultOk(string? email, int column)
    {
        int scheduleId = 1;
        int row = 2;

        var result = await PromtCreateAsync(scheduleId, row, column, email);

        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task CreateAsync_OrderReqDTOAndString_ResultNotFoundException()
    {
        int scheduleId = 100;
        int row = 1;
        int column = 1;
        string? email = null;
        string expected = "Такого киносеанса не существует";

        var result = await PromtCreateAsync(scheduleId, row, column, email);

        Assert.IsType<NotFoundException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(100, 100)]
    public async Task CreateAsync_OrderReqDTOAndString_ResultBadRequestException(int row, int column)
    {
        int scheduleId = 1;
        string? email = null;
        string expected = "Данное место уже занято";

        var result = await PromtCreateAsync(scheduleId, row, column, email);

        Assert.IsType<BadRequestException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion

    #region GetPriceAsync
    //..............Проверка получения цены..............//

    private async Task<ServiceResult<decimal>> PromtGetPriceAsync(Guid guidId)
    {
        decimal discountCoefficient = 1;
        decimal reductionFactorPrice = 1;
        decimal multiplyingFactorPrice = 1;
        int reductionBoundaryTime = 1;
        int multiplyingBoundaryTime = 1;

        return await _orderService.GetPriceAsync(
            guidId, discountCoefficient,
            reductionFactorPrice, multiplyingFactorPrice,
            reductionBoundaryTime, multiplyingBoundaryTime);
    }

    [Fact]
    public async Task GetPriceAsync_GuidDecimalsAndIntegers_ResultOk()
    {
        Guid guidId = Guid.Parse("E3DD38B6-BB92-4283-AB6F-EF30DFE7A8BF");

        var result = await PromtGetPriceAsync(guidId);

        Assert.IsType<decimal>(result.Value);
    }

    [Fact]
    public async Task GetPriceAsync_GuidDecimalsAndIntegers_ResultNotFoundException()
    {
        Guid guidId = Guid.NewGuid();
        string expected = "Данные не доступны";

        var result = await PromtGetPriceAsync(guidId);

        Assert.IsType<NotFoundException>(result.Exception);
        Assert.Equal(expected, result.Error);
    }
    #endregion

    #region ListPositionsAsync
    //..............Проверка получения мест..............//

    [Fact]
    public async Task ListPositionsAsync_Integer_ResultOk()
    {
        int scheduleId = 1;

        var result = await _orderService.ListPositionsAsync(scheduleId);

        Assert.NotNull(result.Value);
    }
    #endregion

    #region ProofOfPaymentAsync
    //..............Проверка подтверждения покупки..............//
    //Добавить проверку выбрасывания исключений

    [Fact]
    public async Task ProofOfPaymentAsync_Guid_ResultOk()
    {
        Guid guidId = Guid.Parse("E3DD38B6-BB92-4283-AB6F-EF30DFE7A8BF");
        string expected = "Ok";

        var result = await _orderService.ProofOfPaymentAsync(guidId);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task ProofOfPaymentAsync_Guid_ResultNotFoundException()
    {
        Guid guidId = Guid.NewGuid();
        string expected = "Данные не доступны";

        var result = await _orderService.ProofOfPaymentAsync(guidId);

        Assert.Equal(expected, result.Error);
    }
    #endregion
}
