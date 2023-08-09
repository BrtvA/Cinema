using Cinema.DAL.Repositories;
using Cinema.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cinema.DAL.UnitOfWorks;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _db;

    public IGenreRepository GenreRepository { get => new GenreRepository(_db); }
    public IMovieRepository MovieRepository { get => new MovieRepository(_db); }
    public IMovieGenreRepository MovieGenreRepository { get => new MovieGenreRepository(_db); }
    public IHallRepository HallRepository { get => new HallRepository(_db); }
    public IScheduleRepository ScheduleRepository { get => new ScheduleRepository(_db); }
    public IOrderRepository OrderRepository { get => new OrderRepository(_db); }
    public IBankAccountRepository BankAccountRepository { get => new BankAccountRepository(_db); }
    public IUserRepository UserRepository { get => new UserRepository(_db); }
    public IUserTypeRepository UserTypeRepository { get => new UserTypeRepository(_db); }

    public UnitOfWork(DbContextOptions<ApplicationContext> dbContextOptions)
    {
        _db = new ApplicationContext(dbContextOptions);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _db.Database.BeginTransactionAsync();
    }

    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool disposed = false;
    public virtual void Dispose(bool disposing)
    {

        if (disposed) return;
        if (disposing)
        {
            _db.Dispose();
        }
        disposed = true;
    }

    ~UnitOfWork()
    {
        Dispose(false);
    }
}
