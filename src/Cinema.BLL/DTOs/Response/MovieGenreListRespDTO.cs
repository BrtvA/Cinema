using Cinema.DAL.Entities;
using Cinema.DAL.Models.Base;

namespace Cinema.BLL.DTOs.Response;

public class MovieGenreListRespDTO : GenreListRespDTO
{
    public IEnumerable<BaseShortModel> MovieList { get; set; }

    public MovieGenreListRespDTO(IEnumerable<BaseShortModel> movieList, IEnumerable<Genre> genreList, bool nextPageExist)
        : base(genreList, nextPageExist)
    {
        MovieList = movieList;
    }
}
