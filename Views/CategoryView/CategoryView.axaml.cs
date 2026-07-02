using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using SistemaDistribuidora.ViewModels;

namespace SistemaDistribuidora.Views;

public partial class CategoryView : UserControl
{
    public CategoryView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<CategoryViewModel>();
    }
}