using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request;

public class AdminScheduleReqDTO : HomeIndexReqDTO
{
    [Range(0, int.MaxValue)]
    public int HallId { get; set; } = 0;
}
