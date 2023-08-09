namespace Cinema.DAL.Entities;

public class Order
{
    public int Id { get; set; }
    public Guid GuidId { get; set; }
    public DateTime CreationDate { get; set; }
    public int ScheduleId { get; set; }
    public int? UserId { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public bool IsPaid { get; set; }

    public User? User { get; set; }
    public Schedule? Schedules { get; set; }
}
