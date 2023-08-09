using Cinema.BLL.DTOs.Request.Base;

namespace Cinema.BLL.DTOs.Request;

public class HomeIndexReqDTO : DateReqDTO
{
    public int[]? GenresId { get; set; }
    public int Page { get; set; } = 1;
}
