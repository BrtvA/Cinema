using Cinema.BLL.CustomExceptions;
using Cinema.BLL.DTOs.Request;
using Cinema.BLL.Services.Interfaces;
using Cinema.DAL.Entities;
using Cinema.DAL.Models;
using Cinema.DAL.Repositories.Interfaces;
using Cinema.DAL.UnitOfWorks;

namespace Cinema.BLL.Services;

public class OrderService : IOrderService
{
    private const int DEFAULT_COEFFICIENT = 1;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IHallRepository _hallRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUserRepository _userRepository;

    private readonly IScheduleService _scheduleService;

    public OrderService(IUnitOfWork unitOfWork, IScheduleService scheduleService)
    {
        _unitOfWork = unitOfWork;
        _hallRepository = unitOfWork.HallRepository;
        _orderRepository = unitOfWork.OrderRepository;
        _scheduleRepository = unitOfWork.ScheduleRepository;
        _userRepository = unitOfWork.UserRepository;

        _scheduleService = scheduleService;
    }

    public async Task<ServiceResult<string>> CreateAsync(OrderReqDTO orderDTO, string? email)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            ServiceResult<string> result;
            var schedule = await _scheduleRepository.GetAsync(orderDTO.ScheduleId);
            if (schedule is null)
            {
                result =  new ServiceResult<string>(
                    new NotFoundException("Такого киносеанса не существует"));
            }
            else
            {
                var hall = await _hallRepository.GetAsync(schedule.HallId);
                if (hall is null)
                {
                    result = new ServiceResult<string>(
                        new NotFoundException("Такого зала не существует"));
                }
                else
                {
                    int? userId = null;
                    if (email is not null)
                    {
                        var user = await _userRepository.GetByEmailAsync(email);
                        userId = user?.Id;
                    }

                    List<Order> orderList = new();
                    bool isContinue = true;
                    Guid guidId = Guid.NewGuid();
                    DateTime currentTime = DateTime.Now;
                    foreach (var position in orderDTO.Positions)
                    {
                        var sch = await _orderRepository.GetByIdAndPositionAsync(orderDTO.ScheduleId, position);

                        if (sch is null && position.Row >= 1 && position.Row <= hall.Rows 
                            && position.Column >= 1 && position.Column <= hall.Columns)
                        {
                            orderList.Add(
                                new Order
                                {
                                    ScheduleId = orderDTO.ScheduleId,
                                    GuidId = guidId,
                                    CreationDate = currentTime,
                                    UserId = userId,
                                    Row = position.Row,
                                    Column = position.Column,
                                    IsPaid = false,
                                }
                            );
                        }
                        else
                        {
                            isContinue = false;
                            break;
                        }
                    }

                    if (isContinue)
                    {
                        await _orderRepository.CreateAsync(orderList);
                        await _unitOfWork.SaveAsync();

                        result = new ServiceResult<string>(guidId.ToString());
                    }
                    else
                    {
                        result = new ServiceResult<string>(
                            new BadRequestException("Данное место уже занято"));
                    }
                }
            }

            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ServiceResult<decimal>> GetPriceAsync(
        Guid guidId, decimal discountCoefficient,
        decimal reductionFactorPrice, decimal multiplyingFactorPrice,
        int reductionBoundaryTime, int multiplyingBoundaryTime)
    {
        var orders = await _orderRepository.ListByGuidAsync(guidId);

        var ticketCount = orders.Count();
        if (ticketCount == 0)
        {
            return new ServiceResult<decimal>(
                new NotFoundException("Данные не доступны"));
        }

        var firstOrder = orders.First();
        return await _scheduleService.GetPriceAsync(
            firstOrder.ScheduleId, ticketCount,
            firstOrder.UserId is not null ? discountCoefficient : DEFAULT_COEFFICIENT,
            reductionFactorPrice, multiplyingFactorPrice,
            reductionBoundaryTime, multiplyingBoundaryTime);
    }

    public async Task<ServiceResult<IEnumerable<PositionModel>>> ListPositionsAsync(int scheduleId)
    {
        var positions = await _orderRepository.ListPositionAsync(scheduleId);
        return new ServiceResult<IEnumerable<PositionModel>>(positions);
    }

    public async Task<ServiceResult<string>> ProofOfPaymentAsync(Guid guidId)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var order = await _orderRepository.GetByGuidAsync(guidId);
            if (order is null)
            {
                return new ServiceResult<string>(
                    new NotFoundException("Данные не доступны"));
            }

            order.IsPaid = true;
            _orderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            await transaction.CommitAsync();
            return new ServiceResult<string>("Ok");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
