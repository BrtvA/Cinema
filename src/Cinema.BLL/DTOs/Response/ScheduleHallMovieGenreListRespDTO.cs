using Cinema.DAL.Entities;
using Cinema.DAL.Models;
using Cinema.DAL.Models.Base;

namespace Cinema.BLL.DTOs.Response;

public class ScheduleHallMovieGenreListRespDTO : GenreListRespDTO
{
    public IEnumerable<ScheduleModel> ScheduleList { get; set; }
    public IEnumerable<BaseShortModel> HallList { get; set; }
    public IEnumerable<BaseShortModel> MovieList { get; set; }
    public ScheduleHallMovieGenreListRespDTO(IEnumerable<ScheduleModel> scheduleList,
                                         IEnumerable<BaseShortModel> hallList,
                                         IEnumerable<BaseShortModel> movieList,
                                         IEnumerable<Genre> genreList, bool nextPageExist)
        : base(genreList, nextPageExist)
    {
        ScheduleList = scheduleList;
        HallList = hallList;
        MovieList = movieList;
    }
}
