using Cinema.BLL.CustomExceptions;
using Cinema.BLL.DTOs.Request;
using Cinema.BLL.DTOs.Response;
using Cinema.BLL.Services.Interfaces;
using Cinema.DAL;
using Cinema.DAL.Entities;
using Cinema.DAL.Enum;
using Cinema.DAL.Repositories.Interfaces;
using Cinema.DAL.UnitOfWorks;
using System.Security.Claims;

namespace Cinema.BLL.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IUserTypeRepository _userTypeRepository;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _userRepository = unitOfWork.UserRepository;
        _userTypeRepository = unitOfWork.UserTypeRepository;
    }

    private static ClaimsIdentity GetClaimsIdentity(string email, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, email),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
        };
        return new ClaimsIdentity(claims, "ApplicationCookie",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
    }

    private static string GetUri(string role) => role switch
    {
        "Admin" => "/genre",
        _ => "/home"
    };

    public async Task<ServiceResult<UserRespDTO>> LoginAsync(LoginReqDTO loginDTO)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            ServiceResult<UserRespDTO> result;
            loginDTO.Trim();
            var user = await _userRepository.GetByEmailAsync(loginDTO.Email);
            if (user is null)
            {
                result = new ServiceResult<UserRespDTO>(
                    new NotFoundException("Пользователь не найден"));
            }
            else if (user.Email == loginDTO.Email && user.Password == Hasher.HashPassword(loginDTO.Password))
            {
                var userType = await _userTypeRepository.GetAsync(user.UserTypeId);
                if (userType is null)
                {
                    result = new ServiceResult<UserRespDTO>(
                        new NotFoundException("Пользователь не найден"));
                }
                else
                {
                    result = new ServiceResult<UserRespDTO>(
                        new UserRespDTO(GetClaimsIdentity(
                                loginDTO.Email, userType.Name),
                                GetUri(userType.Name)));
                }
            }
            else
            {
                result = new ServiceResult<UserRespDTO>(
                    new BadRequestException("Неверные данные"));
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

    public async Task<ServiceResult<UserRespDTO>> RegisterAsync(RegisterReqDTO registerDTO)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            ServiceResult<UserRespDTO> result;
            registerDTO.Trim();
            var user = await _userRepository.GetByEmailAsync(registerDTO.Email);
            if (user is not null)
            {
                result = new ServiceResult<UserRespDTO>(
                    new BadRequestException("Такой пользователь уже существует"));
            }
            else
            {
                await _userRepository.CreateAsync(new User
                {
                    UserTypeId = (int)UserTypeEnum.Customer,
                    Email = registerDTO.Email,
                    Password = Hasher.HashPassword(registerDTO.Password),
                    Name = registerDTO.Name,
                });
                await _unitOfWork.SaveAsync();

                var customerType = UserTypeEnum.Customer.ToString();

                result = new ServiceResult<UserRespDTO>(
                    new UserRespDTO(GetClaimsIdentity(
                            registerDTO.Email, customerType),
                            GetUri(customerType)));
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
