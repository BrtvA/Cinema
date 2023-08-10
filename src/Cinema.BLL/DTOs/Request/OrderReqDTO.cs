using Cinema.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request;

public class OrderReqDTO
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ScheduleId { get; set; }
    [Required]
    public PositionModel[] Positions { get; set; } = null!;
}
