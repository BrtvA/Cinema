using Cinema.BLL.DTOs.Request;
using Cinema.BLL.Services.Interfaces;
using Cinema.DAL.Entities;
using Cinema.PL.Controllers.Extentions;
using Cinema.PL.Enum;
using Cinema.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.PL.Controllers;

[Authorize(Roles = "Admin")]
[Route("")]
public class AdminController : Controller
{
    private readonly int PAGE_SIZE_GENRE;
    private readonly int PAGE_SIZE_MOVIE;
    private readonly int PAGE_SIZE_SCHEDULE;

    private readonly string _uploadPath;

    private readonly IGenreService _genreService;
    private readonly IMovieService _movieService;
    private readonly IScheduleService _scheduleService;

    public AdminController(IGenreService genreService, IMovieService movieService,
                           IScheduleService scheduleService, IWebHostEnvironment environment) {
        _genreService = genreService;
        _movieService = movieService;
        _scheduleService = scheduleService;

        _uploadPath = Path.Combine(environment.WebRootPath, "uploads");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        PAGE_SIZE_GENRE = int.Parse(config
            .GetSection("AppSettings")
            .GetSection("AdminPage")["PageSizeGenre"] ?? "8");
        PAGE_SIZE_MOVIE = int.Parse(config
            .GetSection("AppSettings")
            .GetSection("AdminPage")["PageSizeMovie"] ?? "10");
        PAGE_SIZE_SCHEDULE = int.Parse(config
            .GetSection("AppSettings")
            .GetSection("AdminPage")["PageSizeSchedule"] ?? "3");
    }

    ///..............Афиша..................................
    [HttpGet]
    [Route("/schedule")]
    public async Task<IActionResult> Schedule(AdminScheduleReqDTO scheduleDTO)
    {
        if (scheduleDTO.Date is null || scheduleDTO.Date == "")
        {
            ModelState.Remove("Date");
        }

        if (scheduleDTO.GenresId is null)
        {
            ModelState.Remove("GenresId");
        }

        return await ModelState.ToValidate(async () =>
        {
            var result = await _scheduleService.ListAsync(scheduleDTO, PAGE_SIZE_SCHEDULE);

            return result.ToDo((value) =>
            {
                ViewBag.PageSelect = PageSelectEnum.Schedule;
                ViewBag.PageSize = PAGE_SIZE_SCHEDULE;

                return View("~/Views/Admin/Schedule.cshtml",
                    new ScheduleViewModel(value, scheduleDTO)
                );
            });
        });
    }

    [HttpGet]

    [Route("/schedule-info")]
    public async Task<IActionResult> ScheduleInfo(int id)
    {
        var result = await _scheduleService.GetAsync(id);

        return result.ToDo(Json);
    }

    [HttpPost]
    [Route("/schedule-info")]
    public async Task<IActionResult> ScheduleInfo(ScheduleReqDTO scheduleDTO)
    {
        return await ModelState.ToValidate(async () =>
        {
            var result = await _scheduleService.CreateAsync(scheduleDTO);

            return result.ToDo((value) =>
            {
                return Ok();
            });
        });
    }

    [HttpPut]
    [Route("/schedule-info")]
    public async Task<IActionResult> ScheduleUpdate(ScheduleReqDTO scheduleDTO)
    {
        return await ModelState.ToValidate(async () =>
        {
            var result = await _scheduleService.UpdateAsync(scheduleDTO);

            return result.ToDo((value) =>
            {
                return Ok();
            });
        });
    }

    [HttpDelete]
    [Route("/schedule-info")]
    public async Task<IActionResult> ScheduleDelete(int id)
    {
        var result = await _scheduleService.DeleteAsync(id);

        return result.ToDo((value) =>
        {
            return Ok();
        });
    }


    ///..............Фильмы..................................
    [HttpGet]
    [Route("/movie")]
    public async Task<IActionResult> Movie(AdminMovieReqDTO moviePageDTO)
    {
        if (moviePageDTO.Search is null || moviePageDTO.Search == "")
        {
            ModelState.Remove("Search");
        }

        return await ModelState.ToValidate(async () =>
        {
            var result = await _movieService.ListAsync(moviePageDTO, PAGE_SIZE_MOVIE);

            return result.ToDo((value) =>
            {
                ViewBag.PageSelect = PageSelectEnum.Movie;
                ViewBag.PageSize = PAGE_SIZE_MOVIE;

                return View(
                    "~/Views/Admin/Movie.cshtml",
                    new MovieViewModel(value, moviePageDTO));
            });
        });
    }

    [HttpGet]
    [Route("/movie-info")]
    public async Task<IActionResult> MovieInfo(int id)
    {
        var result = await _movieService.GetAsync(id);

        return result.ToDo(Json);
    }

    [HttpPost]
    [Route("/movie-info")]
    public async Task<IActionResult> MovieInfo(MovieReqDTO movieDTO)
    {
        return await ModelState.ToValidate(async ()=>
        {
            var result = await _movieService.CreateAsync(movieDTO, _uploadPath);

            return result.ToDo((value) =>
            {
                return Ok();
            });
        });
    }

    [HttpPut]
    [Route("/movie-info")]
    public async Task<IActionResult> MovieUpdate(MovieReqDTO movieDTO)
    {
        if (movieDTO.Image is null)
        {
            ModelState.Remove("Image");
        }

        return await ModelState.ToValidate(async () =>
        {
            var result = await _movieService.UpdateAsync(movieDTO, _uploadPath);

            return result.ToDo((value) =>
            {
                return Ok();
            });
        });
    }

    [HttpDelete]
    [Route("/movie-info")]
    public async Task<IActionResult> MovieDelete(int id)
    {
        var result = await _movieService.DeleteAsync(id, _uploadPath);

        return result.ToDo((value) =>
        {
            return Ok();
        });
    }

    ///..............Жанры..................................

    [HttpGet]
    [Route("/genre")]
    [Route("/genre/{page:int}")]
    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _genreService.ListAsync(page, PAGE_SIZE_GENRE);

        return result.ToDo((value) =>
        {
            ViewBag.PageSelect = PageSelectEnum.Genre;
            ViewBag.PageSize = PAGE_SIZE_GENRE;

            return View(
                "~/Views/Admin/Index.cshtml",
                new GenreViewModel(value, page));
        });
    }

    [HttpPost]
    [Route("/genre")]
    public async Task<IActionResult> Genre(GenreReqDTO genreDTO)
    {
        return await ModelState.ToValidate(async () =>
        {
            var result = await _genreService.CreateAsync(genreDTO);

            return result.ToDo((value) =>
            {
                return Ok();
            });
        });
    }

    [HttpPut]
    [Route("/genre")]
    public async Task<IActionResult> Genre(Genre genre)
    {
        return await ModelState.ToValidate(async () =>
        {
            var result = await _genreService.UpdateAsync(genre);

            return result.ToDo((value) =>
            {
                return Ok();
            });
        });
    }

    [HttpDelete]
    [Route("/genre")]
    public async Task<IActionResult> Genre(int id)
    {
        var result = await _genreService.DeleteAsync(id);

        return result.ToDo((value) =>
        {
            return Ok();
        });
    }
}
