using Cinema.BLL.DTOs.Request;
using Cinema.BLL.Services.Interfaces;
using Cinema.PL.Models;
using Microsoft.AspNetCore.Mvc;
using Cinema.PL.Controllers.Extentions;

namespace Cinema.PL.Controllers;

[Route("")]
public class PaymentController : Controller
{
    private readonly bool DOCKER_BUILD;

    private readonly IBankAccountService _bankAccountService;

    public PaymentController(IBankAccountService bankAccountService)
    {
        _bankAccountService = bankAccountService;

        var dockerBuild = Environment.GetEnvironmentVariable("DOCKER_BUILD");
        if (dockerBuild is not null)
        {
            DOCKER_BUILD = bool.Parse(dockerBuild);
        }
        else
        {
            DOCKER_BUILD = false;
        }
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
            string urlString = DOCKER_BUILD
                ? $"{Request.Scheme}://host.docker.internal:{Request.Host.Port}"
                : $"{Request.Scheme}://{Request.Host.ToUriComponent()}";

            //string urlString = $"{Request.Scheme}://host.docker.internal:{Request.Host.Port}";

            var result = await _bankAccountService.BuyAsync(
                    bankAccountDTO,
                    new HttpClient { 
                        BaseAddress = new Uri(urlString),
                    },
                    "/buy-info");

            return result.ToDo((value) =>
            {
                return Redirect($"/proof?guidId={bankAccountDTO.GuidId}");
            });
        });
    }
}
