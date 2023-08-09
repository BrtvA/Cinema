using Cinema.BLL.DTOs;
using Cinema.BLL.DTOs.Response;
using Cinema.DAL.Entities;
using Cinema.DAL.Models;

namespace Cinema.PL.Models;

public class MovieInfoViewModel
{
    public MovieInfoModel MovieInfo { get; set; }
    public IEnumerable<Hall> HallList { get; set; }
    public string Date { get; set; }

    public MovieInfoViewModel(MovieHallListRespDTO dto, string date)
    {
        MovieInfo = dto.MovieInfo;
        HallList = dto.HallList;
        Date = date;
    }
}
