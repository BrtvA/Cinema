using Cinema.BLL.DTOs.Request;
using Cinema.BLL.DTOs.Response;
using Cinema.DAL.Entities;

namespace Cinema.BLL.Services.Interfaces;

public interface IGenreService
{
    public Task<ServiceResult<string>> CreateAsync(GenreReqDTO genreDTO);

    public Task<ServiceResult<GenreListRespDTO>> ListAsync(int page, int pageSize);

    public Task<ServiceResult<string>> UpdateAsync(Genre genreDTO);

    public Task<ServiceResult<string>> DeleteAsync(int id);
}
