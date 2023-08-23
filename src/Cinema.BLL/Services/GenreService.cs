using Cinema.DAL.Repositories.Interfaces;
using Cinema.DAL.Entities;
using Cinema.BLL.CustomExceptions;
using Cinema.DAL.UnitOfWorks;
using Cinema.BLL.Services.Interfaces;
using Cinema.BLL.DTOs.Request;
using Cinema.BLL.DTOs.Response;

namespace Cinema.BLL.Services;

public class GenreService : IGenreService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenreRepository _genreRepository;

    public GenreService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _genreRepository = unitOfWork.GenreRepository;
    }

    public async Task<ServiceResult<string>> CreateAsync(GenreReqDTO genreDTO)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            ServiceResult<string> result;
            genreDTO.Trim();
            var genre = await _genreRepository.GetByNameAsync(genreDTO.Name);
            if (genre is not null)
            {
                result = new ServiceResult<string>(
                    new BadRequestException("Такой жанр уже существует"));
            }
            else
            {
                await _genreRepository.CreateAsync(new Genre { Name = genreDTO.Name });
                await _unitOfWork.SaveAsync();
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

    public async Task<ServiceResult<GenreListRespDTO>> ListAsync(int page, int pageSize)
    {
        var genres = await _genreRepository.ListAsync(pageSize * (page - 1), pageSize);

        int count = await _genreRepository.GetCountAsync();
        bool nextPageExist = (pageSize * page) < count;

        return new ServiceResult<GenreListRespDTO>(
            new GenreListRespDTO(genres, nextPageExist));
    }

    public async Task<ServiceResult<string>> UpdateAsync(Genre genreDTO)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            ServiceResult<string> result;
            genreDTO.Trim();
            var genre = await _genreRepository.GetAsync(genreDTO.Id);
            if (genre is null)
            {
                result = new ServiceResult<string>(
                    new NotFoundException("Такого жанра не существует"));
            }
            else
            {
                genre.Name = genreDTO.Name;
                _genreRepository.Update(genre);
                await _unitOfWork.SaveAsync();
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

    public async Task<ServiceResult<string>> DeleteAsync(int id)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            ServiceResult<string> result;
            var genre = await _genreRepository.GetAsync(id);
            if (genre is null)
            {
                result = new ServiceResult<string>(
                    new NotFoundException("Такого жанра не существует"));
            }
            else
            {
                _genreRepository.Delete(genre);
                await _unitOfWork.SaveAsync();
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
}
