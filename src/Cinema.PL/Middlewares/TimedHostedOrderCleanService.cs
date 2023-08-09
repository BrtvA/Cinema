using Cinema.DAL;
using Cinema.DAL.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cinema.PL.Middlewares;

internal class TimedHostedOrderCleanService : IHostedService, IDisposable
{
    private readonly int RESTART_PERIOD;   // Период запуска очистки, мин
    private readonly int ACTUALITY_PERIOD; // Период актуальности, мин

    private int _executionCount = 0;
    private Timer? _timer = null;

    private readonly ILogger<TimedHostedOrderCleanService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public TimedHostedOrderCleanService(ILogger<TimedHostedOrderCleanService> logger,
                                        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        RESTART_PERIOD = int.Parse(config
            .GetSection("AppSettings")
            .GetSection("Additional")["CleaningRestartPeriod"] ?? "2");
        ACTUALITY_PERIOD = int.Parse(config
            .GetSection("AppSettings")
            .GetSection("Additional")["CleaningActualityPeriod"] ?? "2");
    }

    //private readonly DbContextOptions<ApplicationContext> _dbContextOptions;

    //public TimedHostedOrderCleanService(ILogger<TimedHostedOrderCleanService> logger,
    //                                    DbContextOptions<ApplicationContext> dbContextOptions)
    //{
    //    _logger = logger;
    //    _dbContextOptions = dbContextOptions;
    //}

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Сервис очистки устаревших заказов запущен");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
                           TimeSpan.FromMinutes(RESTART_PERIOD));

        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        var count = Interlocked.Increment(ref _executionCount);

        using (var scope = _scopeFactory.CreateScope())
        {
            var options = scope.ServiceProvider.GetService<DbContextOptions<ApplicationContext>>();

            if (options is not null)
            {
                using (IUnitOfWork unitOfWork = new UnitOfWork(options))
                {
                    var orderRepository = unitOfWork.OrderRepository;

                    using var transaction = await unitOfWork.BeginTransactionAsync();

                    try
                    {
                        var orders = await orderRepository.ListByTimeAsync(ACTUALITY_PERIOD);
                        int deletedCount = orders.Count();
                        if (deletedCount > 0)
                        {
                            orderRepository.Delete(orders);
                            await unitOfWork.SaveAsync();
                        }
                        await transaction.CommitAsync();

                        _logger.LogInformation(
                             $"{DateTime.Now}: Запуск сервиса очистки. Номер запуска: {count}. Удалено: {deletedCount}"
                        );
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            else
            {
                _logger.LogError($"{DateTime.Now}: Невозможность запуска сервиса очистки");
            }
        }
    }

    //private async void DoWork(object? state)
    //{
    //    var count = Interlocked.Increment(ref _executionCount);

    //    using (var scope = _scopeFactory.CreateScope())
    //    {
    //        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    //        var orderRepository = new OrderRepository(dbContext);

    //        using var transaction = await dbContext.Database.BeginTransactionAsync();

    //        try
    //        {
    //            var orders = await orderRepository.ListByTimeAsync(START_TIME);
    //            int deletedCount = orders.Count();
    //            if (deletedCount > 0)
    //            {
    //                orderRepository.Delete(orders);
    //                await orderRepository.SaveAsync();
    //            }
    //            await transaction.CommitAsync();

    //            _logger.LogInformation(
    //                 $"{DateTime.Now}: Запуск сервиса очистки. Номер запуска: {count}. Удалено: {deletedCount}"
    //            );
    //        }
    //        catch
    //        {
    //            await transaction.RollbackAsync();
    //            throw;
    //        }
    //    }
    //}

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Сервис очистки устаревших заказов остановлен");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
        //_db.Dispose();
    }
}
