using Cinema.BLL.DTOs.Request;
using Cinema.BLL.DTOs.Response;
using Cinema.DAL.Entities;
using Cinema.DAL.Models.Base;

namespace Cinema.BLL.Services.Interfaces;

public interface IScheduleService
{
    public Task<ServiceResult<string>> CreateAsync(ScheduleReqDTO scheduleDTO);
    
    public Task<ServiceResult<Schedule>> GetAsync(int id);

    public Task<ServiceResult<decimal>> GetPriceAsync(
        int id, int ticketCount, decimal discountCoefficient,
        decimal reductionFactorPrice, decimal multiplyingFactorPrice,
        int reductionBoundaryTime, int multiplyingBoundaryTime);

    public Task<ServiceResult<ScheduleGenreListRespDTO>> ListInfoAsync(
        HomeIndexReqDTO indexDTO, int pageSize, int dayShift);

    public Task<ServiceResult<ScheduleHallMovieGenreListRespDTO>> ListAsync(
        AdminScheduleReqDTO scheduleDTO, int pageSize);

    public Task<ServiceResult<IEnumerable<BaseShortModel>>> ListTimeAsync(
        HomeTimeReqDTO timeDTO, int dayShift);

    public Task<ServiceResult<string>> UpdateAsync(ScheduleReqDTO scheduleDTO);

    public Task<ServiceResult<string>> DeleteAsync(int id);
}
