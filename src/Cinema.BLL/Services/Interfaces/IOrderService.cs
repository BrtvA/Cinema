using Cinema.BLL.DTOs.Request;
using Cinema.DAL.Models;

namespace Cinema.BLL.Services.Interfaces;

public interface IOrderService
{
    public Task<ServiceResult<string>> CreateAsync(OrderReqDTO orderDTO, string? email);

    public Task<ServiceResult<decimal>> GetPriceAsync(
        Guid guidId, decimal discountCoefficient,
        decimal reductionFactorPrice, decimal multiplyingFactorPrice,
        int reductionBoundaryTime, int multiplyingBoundaryTime);

    public Task<ServiceResult<IEnumerable<PositionModel>>> ListPositionsAsync(int scheduleId);

    public Task<ServiceResult<string>> ProofOfPaymentAsync(Guid guidId);
}
