using Avalonia.Controls;
using SistemaDistribuidora.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace SistemaDistribuidora.Views;

public partial class SupplierView : UserControl
{
    public SupplierView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<SupplierViewModel>();
    }
}