using Cinema.DAL.Models.Base;

namespace Cinema.DAL.Models;

public class MovieModel: BaseShortModel
{
    public string Description { get; set; } = null!;
    public string ImageName { get; set; } = null!;
    public int[] GenresId { get; set; } = null!;
    public int Duration { get; set; }
    public decimal Price { get; set; }
    public bool IsActual { get; set; }
}
