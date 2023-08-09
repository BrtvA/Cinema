using Cinema.BLL.DTOs.Request;
using Cinema.BLL.Services.Interfaces;
using Cinema.PL.Models;
using Microsoft.AspNetCore.Mvc;
using Cinema.PL.Controllers.Extentions;

namespace Cinema.PL.Controllers;

[Route("")]
public class PaymentController : Controller
{
    private readonly IBankAccountService _bankAccountService;

    public PaymentController(IBankAccountService bankAccountService)
    {
        _bankAccountService = bankAccountService;
    }

    [HttpGet]
    [Route("/pay")]
    public IActionResult Index(Guid guidId, decimal price)
    {
        return View(new PaymentViewModel(price, guidId));
    }

    [HttpPost]
    [Route("/pay")]
    public async Task<IActionResult> Index(BankAccountReqDTO bankAccountDTO)
    {
        return await ModelState.ToValidate(async () =>
        {
            var result = await _bankAccountService.BuyAsync(
                    bankAccountDTO,
                    $"{Request.Scheme}://{Request.Host.Value}");

            return result.ToDo((value) =>
            {
                return Redirect($"/proof?guidId={bankAccountDTO.GuidId}");
            });
        });
    }
}
