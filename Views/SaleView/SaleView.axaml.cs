using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using SistemaDistribuidora.ViewModels;

namespace SistemaDistribuidora.Views;
public partial class SaleView : UserControl
{
    public SaleView()
    {
        InitializeComponent();
        InitializeComponent();
        var vm = App.ServiceProvider.GetRequiredService<SaleViewModel>();
        DataContext = vm;
        // Carga clientes y productos al abrir la vista
        vm.InitializeCommand.Execute(null);
    }
}