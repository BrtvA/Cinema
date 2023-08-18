using Cinema.DAL;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Test.Infrastructure.Helpers;

internal class ApplicationContextHelper
{
    public ApplicationContext Context { get; private set; }
    public DbContextOptions<ApplicationContext> Options { get; private set; }

    public ApplicationContextHelper()
    {
        //builder.UseInMemoryDatabase("cinema_test_db")
        //    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        var builder = new DbContextOptionsBuilder<ApplicationContext>();
        builder.UseNpgsql("Host=localhost;Port=5432;Database=cinemadb_test;User Id=postgres;Password=pgpassword;");
        //builder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));

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
