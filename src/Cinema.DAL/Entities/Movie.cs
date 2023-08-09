namespace Cinema.DAL.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ImageName { get; set; } = null!;
    public int Duration { get; set; } //minutes
    public decimal Price { get; set; }
    public bool IsActual { get; set; }

    public List<MovieGenre> MovieGenres { get; set; } = new();
    public List<Schedule> Schedules { get; set; } = new();
}
