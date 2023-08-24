using Cinema.BLL.CustomExceptions;
using Cinema.BLL.DTOs.Request;
using Cinema.BLL.DTOs.Response;
using Cinema.BLL.Services.Additional;
using Cinema.BLL.Services.Interfaces;
using Cinema.DAL.Entities;
using Cinema.DAL.Models;
using Cinema.DAL.Repositories.Interfaces;
using Cinema.DAL.UnitOfWorks;

namespace Cinema.BLL.Services;

public class MovieService : IMovieService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMovieRepository _movieRepository;
    private readonly IMovieGenreRepository _movieGenreRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IHallRepository _hallRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public MovieService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _movieRepository = unitOfWork.MovieRepository;
        _movieGenreRepository = unitOfWork.MovieGenreRepository;
        _genreRepository = unitOfWork.GenreRepository;
        _hallRepository = unitOfWork.HallRepository;
        _scheduleRepository = unitOfWork.ScheduleRepository;
    }

    public async Task<ServiceResult<string>> CreateAsync(MovieReqDTO movieDTO, string uploadPath)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            ServiceResult<string> result;
            movieDTO.Trim();
            var movie = await _movieRepository.GetByNameAsync(movieDTO.Name);
            if (movie is not null)
            {
                result = new ServiceResult<string>(
                    new BadRequestException("Такой фильм уже существует"));
            }
            else if (movieDTO.Image is not null)
            {
                IFileService fileService = new FileService(movieDTO.Image, uploadPath);

                await _movieRepository.CreateAsync(new Movie
                {
                    Name = movieDTO.Name,
                    Description = movieDTO.Description,
                    ImageName = fileService.FileName,
                    Duration = movieDTO.Duration,
                    Price = movieDTO.Price,
                    IsActual = movieDTO.IsActual,
                });
                await _unitOfWork.SaveAsync();

                var currentMovie = await _movieRepository.GetByNameAsync(movieDTO.Name);
                if (currentMovie is null)
                {
                    result = new ServiceResult<string>(
                        new BadRequestException("Добавление нового фильма не удалось"));
                    await transaction.RollbackAsync();
                    return result;
                }

                List<MovieGenre> movieGenreList = new();
                foreach (int id in movieDTO.GenresId)
                {
                    movieGenreList.Add(new MovieGenre
                    {
                        MovieId = currentMovie.Id,
                        GenreId = id
                    });
                }
                await _movieGenreRepository.CreateAsync(movieGenreList);
                await _unitOfWork.SaveAsync();

                await fileService.SaveAsync();

                result = new ServiceResult<string>("Ok");
            }
            else
            {
                result = new ServiceResult<string>("Неизвестная ошибка");
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

    public async Task<ServiceResult<MovieModel>> GetAsync(int id)
    {
        var movie = await _movieRepository.GetInfoAsync(id);
        if (movie is not null)
        {
            return new ServiceResult<MovieModel>(movie);
        }
        else
        {
            return new ServiceResult<MovieModel>(
                new NotFoundException("Такого фильма не существует"));
        }
    }

    public async Task<ServiceResult<MovieHallListRespDTO>> GetDetailsAndHallsAsync(HomeInfoReqDTO infoDTO, int dayShift)
    {
        var timeResult = TimeService.CheckStartTime(infoDTO.Date, dayShift);
        if (timeResult is NotFoundException ex)
        {
            return new ServiceResult<MovieHallListRespDTO>(ex);
        }

        var movie = await _movieRepository.GetActualInfoAsync(infoDTO.MovieId);
        if (movie is null)
        {
            return new ServiceResult<MovieHallListRespDTO>(
                new NotFoundException("Данных не найдено"));
        }

        var halls = await _hallRepository.ListAsync();

        return new ServiceResult<MovieHallListRespDTO>(
            new MovieHallListRespDTO(movie, halls));
    }

    public async Task<ServiceResult<MovieGenreListRespDTO>> ListAsync(AdminMovieReqDTO moviePageDTO, int pageSize)
    {
        moviePageDTO.Trim();

        var movies = await _movieRepository.ListAsync(pageSize * (moviePageDTO.Page - 1), pageSize, moviePageDTO.Search);
        int count = await _movieRepository.GetCountAsync(moviePageDTO.Search);
        var genres = await _genreRepository.ListAsync();

        bool nextPageExist = (pageSize * moviePageDTO.Page) < count;

        return new ServiceResult<MovieGenreListRespDTO>(
            new MovieGenreListRespDTO(movies, genres, nextPageExist));
    }

    public async Task<ServiceResult<string>> UpdateAsync(MovieReqDTO movieDTO, string uploadPath)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            ServiceResult<string> result;
            movieDTO.Trim();
            var movie = await _movieRepository.GetAsync(movieDTO.Id);
            if (movie is null)
            {
                result = new ServiceResult<string>(
                    new NotFoundException("Такого фильма не существует"));
            }
            else
            {
                movie.Name = movieDTO.Name;
                movie.Description = movieDTO.Description;
                movie.Duration = movieDTO.Duration;
                movie.Price = movieDTO.Price;
                movie.IsActual = movieDTO.IsActual;

                string lastImageName = movie.ImageName;
                IFileService? fileService = null;
                if (movieDTO.Image is not null)
                {
                    fileService = new FileService(movieDTO.Image, uploadPath);
                    movie.ImageName = fileService.FileName;
                }

                _movieRepository.Update(movie);
                await _unitOfWork.SaveAsync();

                var mgList = await _movieGenreRepository.ListAsync(movieDTO.Id);
                _movieGenreRepository.Delete(mgList);
                await _unitOfWork.SaveAsync();

                List<MovieGenre> movieGenreList = new();
                foreach (int id in movieDTO.GenresId)
                {
                    movieGenreList.Add(new MovieGenre
                    {
                        MovieId = movieDTO.Id,
                        GenreId = id
                    });
                }
                await _movieGenreRepository.CreateAsync(movieGenreList);
                await _unitOfWork.SaveAsync();

                if (fileService is not null)
                {
                    await fileService.SaveAsync();
                    fileService.FileName = lastImageName;
                    fileService.Delete();
                }

                result = new ServiceResult<string>("Ok");
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

    public async Task<ServiceResult<string>> DeleteAsync(int id, string uploadPath)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            ServiceResult<string> result;
            var movie = await _movieRepository.GetAsync(id);
            if (movie is null)
            {
                result = new ServiceResult<string>(
                    new NotFoundException("Такого фильма не существует"));
            }
            else if (!await _scheduleRepository.ExistByMovieIdAsync(id))
            {
                string fileName = movie.ImageName;
                _movieRepository.Delete(movie);
                await _unitOfWork.SaveAsync();

                IFileService fileService = new FileService(uploadPath, fileName);
                fileService.Delete();

                result = new ServiceResult<string>("Ok");
            }
            else
            {
                result = new ServiceResult<string>(
                    new BadRequestException("Данный фильм используется"));
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
}
