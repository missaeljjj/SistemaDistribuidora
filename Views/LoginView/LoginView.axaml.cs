using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using SistemaDistribuidora.ViewModels;

namespace SistemaDistribuidora.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<LoginViewModel>();
    }
}