using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;

namespace SistemaDistribuidora.Services;

public class NavigationService : INavigationService
{
    private Window? MainWindow =>
        (Application.Current?.ApplicationLifetime
            as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

    /// <summary>
    /// Navega a cualquier vista resolvindola desde el contenedor DI.
    /// </summary>
    public void NavigateTo<TView>() where TView : Control
    {
        var view = App.ServiceProvider.GetRequiredService<TView>();
        if (MainWindow is not null)
            MainWindow.Content = view;
    }

    /// <summary>
    /// Atajo para volver al login (cierre de sesion).
    /// </summary>
    public void NavigateToLogin()
    {
        SessionService.Logout();
        NavigateTo<Views.LoginView>();
    }
}