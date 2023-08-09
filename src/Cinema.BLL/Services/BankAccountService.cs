using Cinema.BLL.CustomExceptions;
using Cinema.BLL.DTOs;
using Cinema.BLL.DTOs.Request;
using Cinema.BLL.Services.Interfaces;
using Cinema.DAL.Repositories.Interfaces;
using Cinema.DAL.UnitOfWorks;
using System.Net.Http.Json;

namespace Cinema.BLL.Services;

public class BankAccountService : IBankAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBankAccountRepository _bankAccountRepository;

    public BankAccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _bankAccountRepository = unitOfWork.BankAccountRepository;
    }

    public async Task<ServiceResult<string>> BuyAsync(BankAccountReqDTO bankAccountDTO, string url)
    {
        var client = new HttpClient();
        var buyInfoDTO = await client.GetFromJsonAsync<BuyInfoDTO>(
            $"{url}/buy-info?guidId={bankAccountDTO.GuidId}&&type=2"
        );

        if (buyInfoDTO is null || buyInfoDTO.CardNumber is null)
        {
            return new ServiceResult<string>(
                new InternalServerErrorException("Ошибка при оплате"));
        }

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            ServiceResult<string> result;
            var clientAccount = await _bankAccountRepository.GetAsync(bankAccountDTO.CardNumber);
            if (clientAccount is null || (clientAccount.DateEnd.Month != bankAccountDTO.MonthEnd
                                       && clientAccount.DateEnd.Year != (2000 + bankAccountDTO.YearEnd)
                                       && clientAccount.Cvc != bankAccountDTO.Cvc))
            {
                result = new ServiceResult<string>(
                    new BadRequestException("Данные о банковской карте не найдены"));
            }
            else if (clientAccount.Money < buyInfoDTO.Price)
            {
                result = new ServiceResult<string>(
                    new BadRequestException("Недостаточное количество денежных средств"));
            }
            else
            {
                var cinemaAccount = await _bankAccountRepository.GetAsync(buyInfoDTO.CardNumber);
                if (cinemaAccount is null)
                {
                    result = new ServiceResult<string>(
                        new BadRequestException("Оплата недоступна"));
                }
                else
                {
                    cinemaAccount.Money += buyInfoDTO.Price;
                    clientAccount.Money -= buyInfoDTO.Price;

                    _bankAccountRepository.Update(cinemaAccount);
                    _bankAccountRepository.Update(clientAccount);
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
}
