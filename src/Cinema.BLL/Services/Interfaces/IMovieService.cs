using Cinema.BLL.DTOs.Request;
using Cinema.BLL.DTOs.Response;
using Cinema.DAL.Models;

namespace Cinema.BLL.Services.Interfaces;

public interface IMovieService
{
    public Task<ServiceResult<string>> CreateAsync(MovieReqDTO movieDTO, string uploadPath);

    public Task<ServiceResult<MovieModel>> GetAsync(int id);

    public Task<ServiceResult<MovieHallListRespDTO>> GetDetailsAndHallsAsync(HomeInfoReqDTO infoDTO, int dayShift);

    public Task<ServiceResult<MovieGenreListRespDTO>> ListAsync(AdminMovieReqDTO moviePageDTO, int pageSize);

    public Task<ServiceResult<string>> UpdateAsync(MovieReqDTO movieDTO, string uploadPath);

    public Task<ServiceResult<string>> DeleteAsync(int id, string uploadPath);
}
