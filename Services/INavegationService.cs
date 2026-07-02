namespace SistemaDistribuidora.Services;

public interface INavigationService
{
    void NavigateTo<TView>() where TView : Avalonia.Controls.Control;
    void NavigateToLogin();
}