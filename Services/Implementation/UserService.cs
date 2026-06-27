using SistemaDistribuidora.DTOs.Auth;
using System.Threading.Tasks;
using SistemaDistribuidora.Repositories.Interfaces;
using SistemaDistribuidora.Services.Interfaces;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Mappers;

namespace SistemaDistribuidora.Services.Implementation;

public class UserService : IUserService
{
    private IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;        
    }

    public async Task<UserSessionDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.AuthenticateAsync(dto.Username, dto.Password);

        if (user is null)
            throw new BussinessRulesException("CredencialesInvalidas", "Usuario o contraseña incorrectos.");

        var session = user.ToUserDto();
        SessionService.Login(session);
        return session;
    }

    public async Task LogOut()
    {
        SessionService.Logout();
    }
}
