using Cinema.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cinema.DAL.UnitOfWorks;

public interface IUnitOfWork : IDisposable
{
    public IGenreRepository GenreRepository { get; }
    public IMovieRepository MovieRepository { get; }
    public IMovieGenreRepository MovieGenreRepository { get; }
    public IHallRepository HallRepository { get; }
    public IScheduleRepository ScheduleRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public IBankAccountRepository BankAccountRepository { get; }
    public IUserRepository UserRepository { get; }
    public IUserTypeRepository UserTypeRepository { get; }

    public Task<IDbContextTransaction> BeginTransactionAsync();
    public Task SaveAsync();
}
