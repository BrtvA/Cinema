using Cinema.DAL.Models.Base;

namespace Cinema.DAL.Models;

public class ScheduleInfoModel : BaseShortModel
{
    public string[] Genres { get; set; } = null!;
    public string ImageName { get; set; } = null!;
}
