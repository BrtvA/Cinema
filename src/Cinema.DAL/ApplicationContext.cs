using Cinema.DAL.Entities;
using Cinema.DAL.Enum;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DAL;

public class ApplicationContext : DbContext
{
    public DbSet<Genre> Genres { get; set; } = null!;
    public DbSet<Movie> Movies { get; set; } = null!;
    public DbSet<MovieGenre> MovieGenres { get; set; } = null!;
    public DbSet<Hall> Halls { get; set; } = null!;
    public DbSet<Schedule> Schedules { get; set; } = null!;
    public DbSet<UserType> UserTypes { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<BankAccount> BankAccounts { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options) :
        base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Genre
        modelBuilder.Entity<Genre>()
                    .HasKey(g => g.Id);
        modelBuilder.Entity<Genre>()
                    .HasIndex(g => g.Id);
        modelBuilder.Entity<Genre>()
                    .HasIndex(g => g.Name);
        modelBuilder.Entity<Genre>()
                    .Property(g => g.Id)
                    .HasColumnType("smallint")
                    .ValueGeneratedOnAdd()
                    .IsRequired();
        modelBuilder.Entity<Genre>()
                    .Property(g => g.Name)
                    .HasColumnType("varchar(30)")
                    .IsRequired();

        //MovieGenre
        modelBuilder.Entity<MovieGenre>()
                    .HasKey(mg => new { mg.MovieId, mg.GenreId });
        modelBuilder.Entity<MovieGenre>()
                    .HasIndex(mg => mg.MovieId);
        modelBuilder.Entity<MovieGenre>()
                    .Property(mg => mg.MovieId)
                    .HasColumnType("smallint")
                    .IsRequired();
        modelBuilder.Entity<MovieGenre>()
                    .Property(mg => mg.GenreId)
                    .HasColumnType("smallint")
                    .IsRequired();

        //Movie
        modelBuilder.Entity<Movie>()
                    .HasKey(m => m.Id);
        modelBuilder.Entity<Movie>()
                    .HasIndex(m => m.Id);
        modelBuilder.Entity<Movie>()
                    .HasIndex(m => m.Name);
        modelBuilder.Entity<Movie>()
                    .HasIndex(m => m.IsActual);
        modelBuilder.Entity<Movie>()
                    .Property(m => m.Id)
                    .HasColumnType("smallint")
                    .ValueGeneratedOnAdd()
                    .IsRequired();
        modelBuilder.Entity<Movie>()
                    .Property(m => m.Name)
                    .HasColumnType("varchar(50)")
                    .IsRequired();
        modelBuilder.Entity<Movie>()
                    .Property(m => m.Description)
                    .HasColumnType("varchar(200)")
                    .IsRequired();
        modelBuilder.Entity<Movie>()
                    .Property(m => m.ImageName)
                    .HasColumnType("varchar(50)")
                    .IsRequired();
        modelBuilder.Entity<Movie>()
                    .Property(m => m.Price)
                    .HasColumnType("money")
                    .IsRequired();
        modelBuilder.Entity<Movie>()
                    .Property(m => m.IsActual)
                    .HasColumnType("boolean")
                    .IsRequired();

        //Hall
        modelBuilder.Entity<Hall>()
                    .HasKey(h => h.Id);
        modelBuilder.Entity<Hall>()
                    .HasIndex(h => h.Id);
        modelBuilder.Entity<Hall>()
                    .Property(h => h.Id)
                    .HasColumnType("smallint")
                    .ValueGeneratedOnAdd()
                    .IsRequired();
        modelBuilder.Entity<Hall>()
                    .Property(h => h.Name)
                    .HasColumnType("varchar(10)")
                    .IsRequired();
        modelBuilder.Entity<Hall>()
                    .Property(h => h.Rows)
                    .HasColumnType("smallint")
                    .IsRequired();
        modelBuilder.Entity<Hall>()
                    .Property(h => h.Columns)
                    .HasColumnType("smallint")
                    .IsRequired();
        modelBuilder.Entity<Hall>().HasData(
            new Hall { Id = 1, Name = "Зал A", Rows = 5, Columns = 5 },
            new Hall { Id = 2, Name = "Зал B", Rows = 5, Columns = 7 },
            new Hall { Id = 3, Name = "Зал C", Rows = 10, Columns = 5 }
        );

        //Schedule
        modelBuilder.Entity<Schedule>()
                    .HasKey(s => s.Id);
        modelBuilder.Entity<Schedule>()
                    .HasIndex(s => s.Id);
        modelBuilder.Entity<Schedule>()
                    .HasIndex(s => new
                    {
                        s.HallId,
                        s.MovieId,
                        s.StartTime
                    });
        modelBuilder.Entity<Schedule>()
                    .Property(s => s.Id)
                    .HasColumnType("smallint")
                    .ValueGeneratedOnAdd()
                    .IsRequired();
        modelBuilder.Entity<Schedule>()
                    .Property(s => s.MovieId)
                    .HasColumnType("int")
                    .IsRequired();
        modelBuilder.Entity<Schedule>()
                    .Property(s => s.HallId)
                    .HasColumnType("smallint")
                    .IsRequired();
        modelBuilder.Entity<Schedule>()
                    .Property(s => s.StartTime)
                    .HasColumnType("timestamp(0)")
                    .IsRequired();

        //Order
        modelBuilder.Entity<Order>()
                    .HasKey(u => u.Id);
        modelBuilder.Entity<Order>()
                    .HasIndex(u => u.GuidId);
        modelBuilder.Entity<Order>()
                    .HasIndex(u => new
                    {
                        u.ScheduleId,
                        u.Row,
                        u.Column,
                    });
        modelBuilder.Entity<Order>()
                    .Property(u => u.Id)
                    .HasColumnType("smallint")
                    .ValueGeneratedOnAdd()
                    .IsRequired();
        modelBuilder.Entity<Order>()
                    .Property(u => u.GuidId)
                    .HasColumnType("uuid")
                    .IsRequired();
        modelBuilder.Entity<Order>()
                   .Property(u => u.CreationDate)
                   .HasColumnType("timestamp(0)")
                   .IsRequired();
        modelBuilder.Entity<Order>()
                   .Property(u => u.ScheduleId)
                   .HasColumnType("int")
                   .IsRequired();
        modelBuilder.Entity<Order>()
                   .Property(u => u.UserId)
                   .HasColumnType("int")
                   .IsRequired(false);
        modelBuilder.Entity<Order>()
                  .Property(u => u.Row)
                  .HasColumnType("int")
                  .IsRequired();
        modelBuilder.Entity<Order>()
                  .Property(u => u.Column)
                  .HasColumnType("int")
                  .IsRequired();
        modelBuilder.Entity<Order>()
                   .Property(u => u.IsPaid)
                   .HasColumnType("boolean")
                   .IsRequired();

        //User
        modelBuilder.Entity<User>()
                    .HasKey(u => u.Id);
        modelBuilder.Entity<User>()
                    .HasIndex(u => u.Id);
        modelBuilder.Entity<User>()
                    .Property(u => u.Id)
                    .HasColumnType("smallint")
                    .ValueGeneratedOnAdd()
                    .IsRequired();
        modelBuilder.Entity<User>()
                    .Property(u => u.Email)
                    .HasColumnType("varchar(50)")
                    .IsRequired();
        modelBuilder.Entity<User>()
                    .Property(u => u.UserTypeId)
                    .HasColumnType("smallint")
                    .IsRequired();
        modelBuilder.Entity<User>()
                    .Property(u => u.Password)
                    .HasColumnType("varchar(64)")
                    .IsRequired();
        modelBuilder.Entity<User>()
                    .Property(u => u.Name)
                    .HasColumnType("varchar(20)")
                    .IsRequired();
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "cinema@yandex.ru",
                UserTypeId = (int)UserTypeEnum.Admin,
                Password = Hasher.HashPassword("1234"),
                Name = "Админ",
            }
        );

        //UserType
        modelBuilder.Entity<UserType>()
                    .HasKey(ut => ut.Id);
        modelBuilder.Entity<UserType>()
                    .HasIndex(ut => ut.Id);
        modelBuilder.Entity<UserType>()
                    .Property(ut => ut.Id)
                    .HasColumnType("smallint")
                    .ValueGeneratedOnAdd()
                    .IsRequired();
        modelBuilder.Entity<UserType>()
                    .Property(ut => ut.Name)
                    .HasColumnType("varchar(20)")
                    .IsRequired();
        modelBuilder.Entity<UserType>().HasData(
            new UserType { Id = 1, Name = UserTypeEnum.Admin.ToString() },
            new UserType { Id = 2, Name = UserTypeEnum.Customer.ToString() }
        );


        //BankAccount
        modelBuilder.Entity<BankAccount>()
            .HasKey(ba => ba.CardNumber);
        modelBuilder.Entity<BankAccount>()
            .HasIndex(ba => ba.CardNumber);
        modelBuilder.Entity<BankAccount>()
            .Property(u => u.CardNumber)
            .HasColumnType("char(16)")
            .IsRequired(true);
        modelBuilder.Entity<BankAccount>()
           .Property(u => u.DateEnd)
           .HasColumnType("date")
           .IsRequired(true);
        modelBuilder.Entity<BankAccount>()
           .Property(u => u.Cvc)
           .HasColumnType("numeric(3)")
           .IsRequired(true);
        modelBuilder.Entity<BankAccount>()
           .Property(u => u.Money)
           .HasColumnType("money")
           .IsRequired(true);
        modelBuilder.Entity<BankAccount>().HasData(
            new BankAccount { 
                CardNumber = "0000000000000000",
                DateEnd = new DateOnly(2030, 1, 1), 
                Cvc = 456,
                Money = 0 
            },
            new BankAccount { 
                CardNumber = "1111222233334444",
                DateEnd = new DateOnly(2030, 1, 1),
                Cvc = 123,
                Money = 100000 
            }
        );
    }
}