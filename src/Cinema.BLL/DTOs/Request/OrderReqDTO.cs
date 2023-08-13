using Cinema.BLL.DTOs.Request.Attributes;
using Cinema.DAL.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request;

public class OrderReqDTO
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ScheduleId { get; set; }
    [Required]
    [ArrayMinValue(1, 1)]
    public PositionModel[] Positions { get; set; } = null!;
}
