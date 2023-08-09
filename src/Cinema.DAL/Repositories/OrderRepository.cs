using Cinema.DAL.Entities;
using Cinema.DAL.Models;
using Cinema.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DAL.Repositories;

public class OrderRepository: IOrderRepository
{
    private readonly ApplicationContext _db;

    public OrderRepository(ApplicationContext applicationContext)
    {
        _db = applicationContext;
    }

    public async Task CreateAsync(List<Order> orderListr)
    {
        await _db.Orders.AddRangeAsync(orderListr);
    }

    private IQueryable<Order> OrderFilteredByGuid(Guid guidId)
    {
        return _db.Orders.Where(o => o.GuidId == guidId);
    }

    public async Task<Order?> GetByGuidAsync(Guid guidId)
    {
        return await OrderFilteredByGuid(guidId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Order>> ListByGuidAsync(Guid guidId)
    {
        return await OrderFilteredByGuid(guidId).ToListAsync();
    }

    private IQueryable<Order> OrderFilteredByScheduleId(int scheduleId)
    {
        return _db.Orders.Where(o => o.ScheduleId == scheduleId);
    }

    public async Task<Order?> GetByIdAndPositionAsync(int scheduleId, PositionModel positionModel)
    {
        return await OrderFilteredByScheduleId(scheduleId)
            .Where(o => o.Row == positionModel.Row && o.Column == positionModel.Column)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<PositionModel>> ListPositionAsync(int scheduleId)
    {
        return await OrderFilteredByScheduleId(scheduleId)
            .Select(o => new PositionModel
            {
                Row = o.Row,
                Column = o.Column,
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> ListByTimeAsync(int minutes)
    {
        return await _db.Orders
            .Where(o => !o.IsPaid
                     && (DateTime.Now - o.CreationDate).TotalMinutes >= minutes)
            .ToListAsync();
    }

    public void Update(Order order)
    {
        _db.Orders.Update(order);
    }

    public void Delete(IEnumerable<Order> orders)
    {
        _db.Orders.RemoveRange(orders);
    }
}
