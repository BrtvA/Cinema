namespace Cinema.DAL.Entities;

public class Schedule
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public int HallId { get; set; }
    //public int DayId { get; set; }
    public DateTime StartTime { get; set; }
    //public TimeOnly StartTime { get; set; }

    public Hall? Hall { get; set; }
    public Movie? Movie { get; set; }
    public List<Order> Orders { get; set; } = new();
}
