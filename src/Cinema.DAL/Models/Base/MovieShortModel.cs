namespace Cinema.DAL.Models.Base;

public class MovieShortModel : BaseShortModel
{
    public string Description { get; set; } = null!;
    public string ImageName { get; set; } = null!;
    public int Duration { get; set; } //minutes
}
