using Cinema.BLL.DTOs;
using Cinema.BLL.DTOs.Request;
using Cinema.BLL.Services.Interfaces;
using Cinema.PL.Controllers.Extentions;
using Cinema.PL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Cinema.PL.Controllers;

[Route("")]
public class HomeController : Controller
{
    private readonly int PAGE_SIZE_SCHEDULE;

    private readonly decimal DISCOUNT_COEFFICIENT;
    private readonly string? CINEMA_CARD_NUMBER;
    private readonly decimal REDUCTION_FACTOR_PRICE;
    private readonly decimal MULTIPLYING_FACTOR_PRICE;
    private readonly int REDUCTION_BOUNDARY_TIME;   //в часах
    private readonly int MULTIPLYING_BOUNDARY_TIME; //в часах

    private readonly int DAY_VISIBILITY_SCOPE;

    private readonly IMovieService _movieService;
    private readonly IScheduleService _scheduleService;
    private readonly IOrderService _orderService;

    public HomeController(IMovieService movieService,
                          IScheduleService scheduleService,
                          IOrderService orderService)
    {
        _movieService = movieService;
        _scheduleService = scheduleService;
        _orderService = orderService;

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        PAGE_SIZE_SCHEDULE = int.Parse(config
            .GetSection("AppSettings")
            .GetSection("CustomerPage")["PageSizeSchedule"] ?? "4");

        DISCOUNT_COEFFICIENT = 1 - Convert.ToDecimal(config
            .GetSection("AppSettings")
            .GetSection("Payment")["DiscountPercentage"] ?? "5") / 100;
        CINEMA_CARD_NUMBER = config
            .GetSection("AppSettings")
            .GetSection("Payment")["CardNumber"];
        REDUCTION_FACTOR_PRICE = 1 - Convert.ToDecimal(config
            .GetSection("AppSettings")
            .GetSection("Payment")["ReductionPercentagePrice"] ?? "10") / 100;
        MULTIPLYING_FACTOR_PRICE = 1 + Convert.ToDecimal(config
            .GetSection("AppSettings")
            .GetSection("Payment")["MultiplyingPercentagePrice"] ?? "10") / 100;
        REDUCTION_BOUNDARY_TIME = int.Parse(config
            .GetSection("AppSettings")
            .GetSection("Payment")["ReductionBoundaryTime"] ?? "10");
        MULTIPLYING_BOUNDARY_TIME = int.Parse(config
            .GetSection("AppSettings")
            .GetSection("Payment")["MultiplyingBoundaryTime"] ?? "16");

        DAY_VISIBILITY_SCOPE = int.Parse(config
            .GetSection("AppSettings")
            .GetSection("Additional")["DayVisibilityScope"] ?? "14");
    }

    [HttpGet]
    [Route("")]
    [Route("/home")]
    public async Task<IActionResult> Index(HomeIndexReqDTO indexDTO)
    {
        if (indexDTO.Date is null || indexDTO.Date == "")
        {
            ModelState.Remove("Date");
        }

        if (indexDTO.GenresId is null)
        {
            ModelState.Remove("GenresId");
        }

        return await ModelState.ToValidate(async () =>
        {
            var result = await _scheduleService.ListInfoAsync(
                    indexDTO, PAGE_SIZE_SCHEDULE,
                    DAY_VISIBILITY_SCOPE);

            return result.ToDo((value) =>
            {
                ViewBag.PageSize = PAGE_SIZE_SCHEDULE;
                ViewBag.IsLoggedIn = User.Identity is not null && User.Identity.IsAuthenticated;
                ViewBag.IsAdmin = User.Identity is not null && User.IsInRole("Admin");

                return View(
                    "~/Views/Home/Index.cshtml",
                    new ScheduleInfoViewModel(value, indexDTO, DAY_VISIBILITY_SCOPE));
            });
        });
    }

    [HttpGet]
    [Route("/info")]
    public async Task<IActionResult> Info(HomeInfoReqDTO infoDTO)
    {
        return await ModelState.ToValidate(async () =>
        {
            var result = await _movieService.GetDetailsAndHallsAsync(infoDTO, DAY_VISIBILITY_SCOPE);

            return result.ToDo((value) =>
            {
                ViewBag.IsLoggedIn = User.Identity is not null && User.Identity.IsAuthenticated;
                ViewBag.IsAdmin = User.Identity is not null && User.IsInRole("Admin");

                return View("~/Views/Home/Info.cshtml", new MovieInfoViewModel(value, infoDTO.Date));
            });
        });
    }

    [HttpPost]
    [Route("/info")]
    public async Task<IActionResult> Info(OrderReqDTO orderDTO)
    {
        return await ModelState.ToValidate(async () =>
        {
            var result = await _orderService.CreateAsync(orderDTO, User.Identity?.Name);

            return result.ToDo((value) =>
            {
                return Json(new { uri = "/buy-info?guidId=", guidId = value });
            });
        });
    }

    [HttpGet]
    [Route("/time")]
    public async Task<IActionResult> Time(HomeTimeReqDTO timeDTO)
    {
        return await ModelState.ToValidate(async () =>
        {
            var result = await _scheduleService.ListTimeAsync(timeDTO, DAY_VISIBILITY_SCOPE);

            return result.ToDo(Json);
        });
    }

    [HttpGet]
    [Route("/price")]
    public async Task<IActionResult> Price(int scheduleId, int ticketCount)
    {
        decimal discount = (User.Identity is not null && User.Identity.IsAuthenticated) 
            ? DISCOUNT_COEFFICIENT : 1;

        var result = await _scheduleService.GetPriceAsync(
                scheduleId, ticketCount, discount,
                REDUCTION_FACTOR_PRICE, MULTIPLYING_FACTOR_PRICE,
                REDUCTION_BOUNDARY_TIME, MULTIPLYING_BOUNDARY_TIME);

        return result.ToDo((value) =>
        {
            return Json(value);
        });
    }

    [HttpGet]
    [Route("/positions")]
    public async Task<IActionResult> Positions(int scheduleId)
    {
        var result = await _orderService.ListPositionsAsync(scheduleId);

        return result.ToDo(Json);
    }

    ///////////////////

    [HttpGet]
    [Route("/buy-info")]
    public async Task<IActionResult> BuyInfo(Guid guidId, int type = 0)
    {
        var result = await _orderService.GetPriceAsync(
                guidId, DISCOUNT_COEFFICIENT,
                REDUCTION_FACTOR_PRICE, MULTIPLYING_FACTOR_PRICE,
                REDUCTION_BOUNDARY_TIME, MULTIPLYING_BOUNDARY_TIME);

        return result.ToDo((value) =>
        {
            //Получить информацию
            if (type == 0)
            {
                return Redirect($"/pay?guidId={guidId}&&price={value}");
            }
            //Оплатить
            else
            {
                return Json(new BuyInfoDTO(CINEMA_CARD_NUMBER, value));
            }
        });
    }

    [HttpGet]
    [Route("/proof")]
    public async Task<IActionResult> Proof(Guid guidId)
    {
        var result = await _orderService.ProofOfPaymentAsync(guidId);

        return result.ToDo((value) =>
        {
            return Ok("/home");
        });
    }

    ///////////////////

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}