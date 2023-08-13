using Cinema.DAL.Models.Base;

namespace Cinema.DAL.Entities;

public class Movie : MovieShortModel
{
    public decimal Price { get; set; }
    public bool IsActual { get; set; }

    public List<MovieGenre> MovieGenres { get; set; } = new();
    public List<Schedule> Schedules { get; set; } = new();
}
