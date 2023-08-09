using Cinema.BLL.DTOs.Request;
using Cinema.BLL.DTOs.Response;
using Cinema.DAL.Models;
using Cinema.DAL.Models.Base;

namespace Cinema.PL.Models;

public class ScheduleViewModel : GenreViewModel
{
    public IEnumerable<ScheduleModel> ScheduleList { get; set; }
    public IEnumerable<BaseShortModel> HallList { get; set; }
    public IEnumerable<BaseShortModel> MovieList { get; set; }
    public string Date { get; set; }
    public int[]? GenresId { get; set; }
    public int HallId { get; set; }

    public ScheduleViewModel(ScheduleHallMovieGenreListRespDTO listDto, AdminScheduleReqDTO scheduleDTO)
        : base(listDto.GenreList, scheduleDTO.Page, listDto.NextPageExist)
    {
        ScheduleList = listDto.ScheduleList;
        HallList = listDto.HallList;
        MovieList = listDto.MovieList;
        Date = scheduleDTO.Date;
        GenresId = scheduleDTO.GenresId;
        HallId = scheduleDTO.HallId;
    }
}
