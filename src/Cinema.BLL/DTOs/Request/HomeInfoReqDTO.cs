using Cinema.BLL.DTOs.Request.Base;
using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request;

public class HomeInfoReqDTO : DateReqDTO
{
    [Range(1, int.MaxValue)]
    public int MovieId { get; set; }
}
