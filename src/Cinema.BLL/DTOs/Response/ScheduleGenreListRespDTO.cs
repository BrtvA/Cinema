using Cinema.DAL.Entities;
using Cinema.DAL.Models;

namespace Cinema.BLL.DTOs.Response
{
    public class ScheduleGenreListRespDTO : GenreListRespDTO
    {
        public IEnumerable<ScheduleInfoModel> ScheduleList { get; set; }
        public ScheduleGenreListRespDTO(IEnumerable<ScheduleInfoModel> scheduleList,
            IEnumerable<Genre> genreList, bool nextPageExist)
            : base(genreList, nextPageExist)
        {
            ScheduleList = scheduleList;
        }
    }
}
