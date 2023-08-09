namespace Cinema.DAL.Entities;

public class Hall
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Rows { get; set; }
    public int Columns { get; set; }

    public List<Schedule> Schedules { get; set; } = new();
}