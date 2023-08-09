using Cinema.DAL.Entities;
using Cinema.DAL.Models;

namespace Cinema.BLL.DTOs.Response;

public class MovieHallListRespDTO
{
    public MovieInfoModel MovieInfo { get; set; }
    public IEnumerable<Hall> HallList { get; set; }

    public MovieHallListRespDTO(MovieInfoModel movieInfo,
                            IEnumerable<Hall> hallList)
    {
        MovieInfo = movieInfo;
        HallList = hallList;
    }
}
