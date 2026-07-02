using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using SistemaDistribuidora.ViewModels;

namespace SistemaDistribuidora.Views;

public partial class ProductView : UserControl
{
    public ProductView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<ProductViewModel>();
    }
}