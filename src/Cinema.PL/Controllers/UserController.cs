using Cinema.BLL.DTOs.Request;
using Cinema.BLL.Services.Interfaces;
using Cinema.PL.Controllers.Extentions;
using Cinema.PL.Enum;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cinema.PL.Controllers;

[Route("")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("/login")]
    public IActionResult Login()
    {
        ViewBag.IsLoggedIn = User.Identity is not null && User.Identity.IsAuthenticated;
        ViewBag.IsAdmin = User.Identity is not null && User.IsInRole("Admin");
        ViewBag.Type = EnterTypeEnum.Login;

        return View("~/Views/User/Index.cshtml");
    }

    [HttpPost]
    [Route("/login")]
    public async Task<IActionResult> Login(LoginReqDTO loginDTO)
    {
        return await ModelState.ToValidate(async () =>
        {
            var result = await _userService.LoginAsync(loginDTO);

            return await result.ToDoAsync(async (value) => {
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(value.ClaimsIdentity));
                return Ok(value.Uri);
            });
        });
    }

    [HttpGet]
    [Route("/register")]
    public IActionResult Register()
    {
        ViewBag.IsLoggedIn = User.Identity is not null && User.Identity.IsAuthenticated;
        ViewBag.IsAdmin = User.Identity is not null && User.IsInRole("Admin");
        ViewBag.Type = EnterTypeEnum.Register;

        return View("~/Views/User/Index.cshtml");
    }

    [HttpPost]
    [Route("/register")]
    public async Task<IActionResult> Register(RegisterReqDTO registerDTO)
    {
        return await ModelState.ToValidate(async () =>
        {
            var result = await _userService.RegisterAsync(registerDTO);

            return await result.ToDoAsync(async (value) => {
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(value.ClaimsIdentity));
                return Ok(value.Uri);
            });
        });
    }

    [HttpGet]
    [Route("/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }
}
