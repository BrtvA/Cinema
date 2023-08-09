using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request;

public class ScheduleReqDTO
{
    [Required]
    public int Id { get; set; }
    [Required]
    public int MovieId { get; set; }
    [Required]
    public int HallId { get; set; }
    [Required]
    public DateTime StartTime { get; set; }
}
