using Cinema.DAL.Entities;

namespace Cinema.BLL.DTOs.Response;

public class GenreListRespDTO
{
    public IEnumerable<Genre> GenreList { get; set; }
    public bool NextPageExist { get; set; }

    public GenreListRespDTO(IEnumerable<Genre> genreList, bool nextPageExist)
    {
        GenreList = genreList;
        NextPageExist = nextPageExist;
    }
}
