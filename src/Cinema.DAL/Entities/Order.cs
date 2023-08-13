using Cinema.DAL.Models.Base;

namespace Cinema.DAL.Entities;

public class Order : PositionModel
{
    public int Id { get; set; }
    public Guid GuidId { get; set; }
    public DateTime CreationDate { get; set; }
    public int ScheduleId { get; set; }
    public int? UserId { get; set; }
    public bool IsPaid { get; set; }

    public User? User { get; set; }
    public Schedule? Schedule { get; set; }
}
