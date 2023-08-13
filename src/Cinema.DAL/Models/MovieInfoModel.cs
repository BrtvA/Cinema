using Cinema.DAL.Models.Base;

namespace Cinema.DAL.Models;

public class MovieInfoModel : MovieShortModel
{
    public string[] Genres { get; set; } = null!;
}
