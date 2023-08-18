using Cinema.DAL.Entities;

namespace Cinema.Test.Infrastructure.Helpers;

public class DataHelper
{
    public static IEnumerable<Genre> GenreList =>
        Enumerable.Range(1, 10).Select(g => new Genre
        {
            Name = $"Тестовый {g}"
        });

    public static IEnumerable<Movie> MovieList =>
        Enumerable.Range(1, 10).Select(m => new Movie
        {
            Name = $"Тестовый фильм {m}",
            Description = "Описание",
            ImageName = "image.png",
            Duration = 10,
            Price = 300,
            IsActual = true,
        });

    public static IEnumerable<MovieGenre> MovieGenreList => new List<MovieGenre>
    {
        new MovieGenre { MovieId = 1, GenreId = 1 },
        new MovieGenre { MovieId = 1, GenreId = 2 },
        new MovieGenre { MovieId = 2, GenreId = 1 },
        new MovieGenre { MovieId = 2, GenreId = 2 },
        new MovieGenre { MovieId = 3, GenreId = 1 },
        new MovieGenre { MovieId = 3, GenreId = 2 },
        new MovieGenre { MovieId = 4, GenreId = 1 },
        new MovieGenre { MovieId = 4, GenreId = 2 },
    };

    public static IEnumerable<Schedule> ScheduleList 
    { 
        get {
            var date = DateTime.Now;
            return Enumerable.Range(1, 5).Select(s => new Schedule
            {
                MovieId = 1,
                HallId = 1,
                StartTime = date.AddMinutes((s - 1) * 10),
            });
        } 
    }

    public static IEnumerable<Order> OrderList 
    { 
        get
        {
            string[] strGuidArray = new[]
            {
                "E3DD38B6-BB92-4283-AB6F-EF30DFE7A8BF",
                "E9D7F26F-24B8-4736-BE56-0B7ACDE509CF",
                "F573912F-0A2B-42D7-A7BF-E776531C369C"
            };

            return Enumerable.Range(1, strGuidArray.Length).Select(num => new Order
            {
                GuidId = Guid.Parse(strGuidArray[num-1]),
                CreationDate = DateTime.Now,
                ScheduleId = 1,
                IsPaid = false,
                Row = 1,
                Column = num,
            });
        } 
    }
}
