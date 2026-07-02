using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using SistemaDistribuidora.ViewModels;

namespace SistemaDistribuidora.Views;

public partial class EmployeeView : UserControl
{
    public EmployeeView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetService<EmployeeViewModel>();
    }
}