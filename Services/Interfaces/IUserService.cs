using SistemaDistribuidora.DTOs.Auth;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Services.Interfaces;

public interface IUserService
{
    Task<UserSessionDto?> LoginAsync(LoginDto loginDto);

    Task LogOut();
}
