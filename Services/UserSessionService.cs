using SistemaDistribuidora.DTOs.Auth;

namespace SistemaDistribuidora.Services;

public static class SessionService
{
    private static UserSessionDto? _currentSession;

    public static UserSessionDto? CurrentSession => _currentSession;

    public static bool IsAuthenticated => _currentSession is not null;

    public static void Login(UserSessionDto session)
    {
        _currentSession = session;
    }

    public static void Logout()
    {
        _currentSession = null;
    }

    // Helpers para no estar accediendo a CurrentSession?.Role en todos lados
    public static bool IsAdmin => _currentSession?.Role == "Admin";
    public static bool IsSeller => _currentSession?.Role == "Vendedor";
}