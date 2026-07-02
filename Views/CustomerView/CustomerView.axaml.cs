using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using SistemaDistribuidora.ViewModels;

namespace SistemaDistribuidora.Views;

public partial class CustomerView : UserControl
{
    public CustomerView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<CustomerViewModel>();
    }
}