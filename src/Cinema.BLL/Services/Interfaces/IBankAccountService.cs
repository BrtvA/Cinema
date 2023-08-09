using Cinema.BLL.DTOs.Request;

namespace Cinema.BLL.Services.Interfaces;

public interface IBankAccountService
{
    public Task<ServiceResult<string>> BuyAsync(BankAccountReqDTO bankAccountDTO, string url);
}
