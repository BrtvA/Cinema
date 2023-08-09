using Cinema.DAL.Entities;
using Cinema.DAL.Models;

namespace Cinema.DAL.Repositories.Interfaces;

public interface IOrderRepository
{
    public Task CreateAsync(List<Order> orderList);
    public Task<Order?> GetByGuidAsync(Guid guidId);
    public Task<Order?> GetByIdAndPositionAsync(int scheduleId, PositionModel positionModel);
    public Task<IEnumerable<PositionModel>> ListPositionAsync(int scheduleId);
    public Task<IEnumerable<Order>> ListByTimeAsync(int minutes);
    public Task<IEnumerable<Order>> ListByGuidAsync(Guid guidId);
    public void Update(Order order);
    public void Delete(IEnumerable<Order> orders);
}
