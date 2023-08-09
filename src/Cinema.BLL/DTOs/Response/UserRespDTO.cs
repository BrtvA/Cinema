using System.Security.Claims;

namespace Cinema.BLL.DTOs.Response;

public class UserRespDTO
{
    public ClaimsIdentity ClaimsIdentity { get; set; } = null!;
    public string Uri { get; set; } = null!;

    public UserRespDTO(ClaimsIdentity claimsIdentity, string uri)
    {
        ClaimsIdentity = claimsIdentity;
        Uri = uri;
    }
}
