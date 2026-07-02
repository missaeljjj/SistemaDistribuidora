using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaDistribuidora.DTOs.Auth;
using SistemaDistribuidora.Services;
using SistemaDistribuidora.Services.Interfaces;
using SistemaDistribuidora.Views;
using System;
using System.Threading.Tasks;

namespace SistemaDistribuidora.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUserService _userService;
    private readonly INavigationService _nav;

    [ObservableProperty]
    private string _username = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _isLoading = false;

    public LoginViewModel(IUserService userService, INavigationService nav)
    {
        _userService = userService;
        _nav = nav;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = "";

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Por favor ingrese usuario y contraseña.";
            return;
        }

        IsLoading = true;
        try
        {
            var dto = new LoginDto { Username = Username, Password = Password };
            var session = await _userService.LoginAsync(dto);

            if (session is null)
            {
                ErrorMessage = "Usuario o contraseña incorrectos.";
                return;
            }

            SessionService.Login(session);
            _nav.NavigateTo<MainMenuView>();   
        }
        catch(Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";

        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ExitApplication()
    {
        //Cierra aplicacion
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
}