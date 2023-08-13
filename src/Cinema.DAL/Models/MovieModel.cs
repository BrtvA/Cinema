using Cinema.DAL.Models.Base;

namespace Cinema.DAL.Models;

public class MovieModel : MovieShortModel
{
    public int[] GenresId { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsActual { get; set; }
}
