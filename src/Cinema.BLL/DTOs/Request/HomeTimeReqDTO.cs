using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request;

public class HomeTimeReqDTO : HomeInfoReqDTO
{
    [Range(1, int.MaxValue)]
    public int HallId { get; set; }
}
