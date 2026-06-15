using SistemaDistribuidora.Models;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> AuthenticateAsync(string User, string Password);
}
