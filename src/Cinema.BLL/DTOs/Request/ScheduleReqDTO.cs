using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request;

public class ScheduleReqDTO
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Id { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int MovieId { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int HallId { get; set; }
    [Required]
    public DateTime StartTime { get; set; }
}
