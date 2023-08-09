using Cinema.DAL.Models.Base;

namespace Cinema.DAL.Models;

public class MovieInfoModel : BaseShortModel
{
    public string Description { get; set; } = null!;
    public string ImageName { get; set; } = null!;
    public string[] Genres { get; set; } = null!;
    public int Duration { get; set; }
}
