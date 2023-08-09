using Cinema.BLL.DTOs.Request;
using Cinema.BLL.DTOs.Response;
using Cinema.DAL.Models;

namespace Cinema.PL.Models;

public class ScheduleInfoViewModel : GenreViewModel
{
    public IEnumerable<ScheduleInfoModel> ScheduleList { get; set; }
    public string Date { get; set; }
    public int[]? GenresId { get; set; }
    public int DayShift { get; set; }

    public ScheduleInfoViewModel(ScheduleGenreListRespDTO dto, HomeIndexReqDTO indexDTO, int dayShift)
        : base(dto.GenreList, indexDTO.Page, dto.NextPageExist)
    {
        ScheduleList = dto.ScheduleList;
        Date = indexDTO.Date;
        GenresId = indexDTO.GenresId;
        DayShift = dayShift;
    }
}
