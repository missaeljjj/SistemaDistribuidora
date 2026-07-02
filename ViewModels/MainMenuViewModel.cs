    using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaDistribuidora.Services;
using SistemaDistribuidora.Views;


namespace SistemaDistribuidora.ViewModels;

public partial class MainMenuViewModel : ObservableObject
{
    private readonly INavigationService _nav;

    // ── Info de sesion
    public string WelcomeMessage =>
        $"Hola, {SessionService.CurrentSession?.Username ?? "Usuario"}";

    public string UserRole =>
        SessionService.CurrentSession?.Role ?? "";

    // ── Visibilidad por rol
    // Solo el Admin ve la gestión de empleados
    public bool IsAdmin => SessionService.IsAdmin;

    public MainMenuViewModel(INavigationService nav)
    {
        _nav = nav;
    }


    [RelayCommand]
    public void GoToCategories() => _nav.NavigateTo<CategoryView>();

    [RelayCommand]
    public void GoToProducts() => _nav.NavigateTo<ProductView>();

    [RelayCommand]
    public void GoToCustomers() => _nav.NavigateTo<CustomerView>();

    [RelayCommand]
    public void GoToSuppliers() => _nav.NavigateTo<SupplierView>();

    [RelayCommand]
    public void GoToEmployees() => _nav.NavigateTo<EmployeeView>();

    [RelayCommand]
    public void GoToSales() => _nav.NavigateTo<SaleView>();

    [RelayCommand]
    public void GoToPurchases() => _nav.NavigateTo<PurchaseView>();

    [RelayCommand]
    public void LogOut() => _nav.NavigateToLogin(); 


}