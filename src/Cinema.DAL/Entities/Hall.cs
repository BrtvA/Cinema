using Cinema.DAL.Models.Base;

namespace Cinema.DAL.Entities;

public class Hall : BaseShortModel
{
    public int Rows { get; set; }
    public int Columns { get; set; }

    public List<Schedule> Schedules { get; set; } = new();
}