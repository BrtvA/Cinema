using Cinema.BLL.DTOs.Request;
using Cinema.BLL.DTOs.Response;

namespace Cinema.BLL.Services.Interfaces;

public interface IUserService
{
    public Task<ServiceResult<UserRespDTO>> LoginAsync(LoginReqDTO loginDTO);

    public Task<ServiceResult<UserRespDTO>> RegisterAsync(RegisterReqDTO registerDTO);
}
