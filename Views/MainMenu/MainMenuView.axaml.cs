using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using SistemaDistribuidora.ViewModels;

namespace SistemaDistribuidora.Views;

public partial class MainMenuView : UserControl
{
    public MainMenuView()
    {
        InitializeComponent();

        DataContext = App.ServiceProvider.GetRequiredService<MainMenuViewModel>();
    }
}