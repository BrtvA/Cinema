namespace Cinema.DAL.Models;

public class ScheduleModel : ScheduleInfoModel
{
    public string HallName { get; set; } = null!;
    public DateTime StartTime { get; set; }
}
