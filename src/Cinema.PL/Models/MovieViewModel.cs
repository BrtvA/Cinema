using Cinema.BLL.DTOs.Request;
using Cinema.BLL.DTOs.Response;
using Cinema.DAL.Models.Base;

namespace Cinema.PL.Models;

public class MovieViewModel : GenreViewModel
{
    public IEnumerable<BaseShortModel> MovieList { get; set; }
    public string Search { get; set; }

    public MovieViewModel(MovieGenreListRespDTO dto, AdminMovieReqDTO moviePageDTO)
        :base(dto.GenreList, moviePageDTO.Page, dto.NextPageExist)
    {
        Search = moviePageDTO.Search;
        MovieList = dto.MovieList;
    }
}
