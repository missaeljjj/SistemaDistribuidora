using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using SistemaDistribuidora.ViewModels;

namespace SistemaDistribuidora.Views;

public partial class PurchaseView : UserControl
{
    public PurchaseView()
    {
        InitializeComponent();
        var vm = App.ServiceProvider.GetRequiredService<PurchaseViewModel>();
        DataContext = vm;
        vm.InitializeCommand.Execute(null);
    }
}