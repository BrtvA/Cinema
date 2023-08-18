using Cinema.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Cinema.Test.Infrastructure.Helpers;

internal class ApplicationContextHelper
{
    public ApplicationContext Context { get; private set; }
    public DbContextOptions<ApplicationContext> Options { get; private set; }

    public ApplicationContextHelper()
    {
        var config = new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .Build();

        var builder = new DbContextOptionsBuilder<ApplicationContext>();
        builder.UseNpgsql(config.GetSection("ConnectionStrings")["CinemaDBTest"]);

        Options = builder.Options;
        Context = new ApplicationContext(Options);

        if (Context.Database.EnsureCreated())
        {
            Context.Genres.AddRange(DataHelper.GenreList);
            Context.Movies.AddRange(DataHelper.MovieList);
            Context.SaveChanges();

            Context.MovieGenres.AddRange(DataHelper.MovieGenreList);
            Context.SaveChanges();

            Context.Schedules.AddRange(DataHelper.ScheduleList);
            Context.SaveChanges();

            Context.Orders.AddRange(DataHelper.OrderList);
            Context.SaveChanges();
        }
    }
}
