using Cinema.BLL.DTOs.Response;
using Cinema.DAL.Entities;

namespace Cinema.PL.Models;

public class GenreViewModel
{
    public IEnumerable<Genre> GenreList { get; set; }
    public int Page { get; set; }
    public bool NextPageExist { get; set; }

    public GenreViewModel(GenreListRespDTO genreListDTO, int page)
    {
        Page = page;
        NextPageExist = genreListDTO.NextPageExist;
        GenreList = genreListDTO.GenreList;
    }

    public GenreViewModel(IEnumerable<Genre> genreList, int page, bool nextPageExist)
    {
        GenreList = genreList;
        Page = page;
        NextPageExist = nextPageExist;
    }
}
