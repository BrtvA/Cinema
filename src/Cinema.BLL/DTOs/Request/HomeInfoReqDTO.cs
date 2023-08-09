using Cinema.BLL.DTOs.Request.Base;

namespace Cinema.BLL.DTOs.Request;

public class HomeInfoReqDTO : DateReqDTO
{
    public int MovieId { get; set; }
}
