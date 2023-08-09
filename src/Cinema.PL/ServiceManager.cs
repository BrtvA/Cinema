using Cinema.BLL.Services;
using Cinema.BLL.Services.Interfaces;
using Cinema.DAL;
using Cinema.DAL.Repositories;
using Cinema.DAL.Repositories.Interfaces;
using Cinema.DAL.UnitOfWorks;
using Cinema.PL.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Cinema.PL;

internal static class ServiceManager
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie(options =>
                        {
                            options.LoginPath = "/login";
                            options.AccessDeniedPath = "/login";
                        });
        builder.Services.AddAuthorization();

        builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("CinemaDB")));

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddScoped<IMovieRepository, MovieRepository>();
        builder.Services.AddScoped<IGenreRepository, GenreRepository>();
        builder.Services.AddScoped<IMovieGenreRepository, MovieGenreRepository>();
        builder.Services.AddScoped<IHallRepository, HallRepository>();
        builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
        builder.Services.AddScoped<IUserTypeRepository, UserTypeRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddScoped<IMovieService, MovieService>();
        builder.Services.AddScoped<IGenreService, GenreService>();
        builder.Services.AddScoped<IScheduleService, ScheduleService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IBankAccountService, BankAccountService>();
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddHostedService<TimedHostedOrderCleanService>();
    }
}