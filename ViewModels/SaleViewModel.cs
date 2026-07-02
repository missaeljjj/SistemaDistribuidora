using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Services;
using SistemaDistribuidora.Services.Interfaces;
using SistemaDistribuidora.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDistribuidora.ViewModels;

public partial class SaleViewModel : ObservableObject
{
    private readonly INavigationService _nav;
    private readonly ISaleService _saleService;
    private readonly IProductService _productService;
    private readonly ICustomerService _customerService;

    // Catalogo completo en memoria para el buscador
    private List<InventoryDetailDto> _allProducts = new();

    public string WelcomeMessage => $"Hola, {SessionService.CurrentSession?.Username ?? "Usuario"}";
    public string UserRole => SessionService.CurrentSession?.Role ?? "";

    public SaleViewModel(
        INavigationService nav,
        ISaleService saleService,
        IProductService productService,
        ICustomerService customerService)
    {
        _nav = nav;
        _saleService = saleService;
        _productService = productService;
        _customerService = customerService;

        ShowWelcome = true;
        ShowRegisterSale = false;
    }

    // Paneles 
    [ObservableProperty] private bool _showWelcome;
    [ObservableProperty] private bool _showRegisterSale;

    [RelayCommand]
    public void ShowRegisterSalePanel()
    {
        ResetPanels();
        ShowRegisterSale = true;
    }

    private void ResetPanels()
    {
        ShowWelcome = false;
        ShowRegisterSale = false;
        ErrorMessage = "";
        SuccessMessage = "";
    }

    // Estado UI 
    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private string _successMessage = "";

    // Clientes
    [ObservableProperty] private ObservableCollection<CustomerListDto> _customers = new();
    [ObservableProperty] private CustomerListDto? _selectedCustomer;

    // Buscador de productos
    [ObservableProperty] private string _productSearch = "";
    [ObservableProperty] private ObservableCollection<InventoryDetailDto> _filteredProducts = new();
    [ObservableProperty] private InventoryDetailDto? _selectedProduct;
    [ObservableProperty] private int _quantity = 1;
    [ObservableProperty] private decimal _unitPrice = 0;

    // Carrito 
    [ObservableProperty] private ObservableCollection<CartItemViewModel> _cartItems = new();
    [ObservableProperty] private decimal _totalAmount = 0;

    // Navegacion
    [RelayCommand]
    public void GoToMainMenu() => _nav.NavigateTo<MainMenuView>();

    //  Inicializacion
    [RelayCommand]
    public async Task InitializeAsync()
    {
        IsLoading = true;
        ErrorMessage = "";
        try
        {
            // Cargar clientes
            var customers = await _customerService.GetAllCustomers();
            Customers.Clear();
            foreach (var c in customers) Customers.Add(c);

            // Cargar productos en memoria para el buscador
            var products = await _productService.GetAllProducts();
            _allProducts = products.ToList();
        }
        catch
        {
            ErrorMessage = "Error al cargar los datos.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Buscador
    partial void OnProductSearchChanged(string value)
    {
        FilteredProducts.Clear();
        if (string.IsNullOrWhiteSpace(value)) return;

        var matches = _allProducts
            .Where(p => p.ProductName.Contains(value, StringComparison.OrdinalIgnoreCase)
                     && p.Stock > 0 && p.SalePrice > 0).Take(6);

        foreach (var p in matches)
            FilteredProducts.Add(p);
    }

    [RelayCommand]
    private void SelectProduct(InventoryDetailDto product)
    {
        SelectedProduct = product;
        ProductSearch = product.ProductName;
        Quantity = 1;
        UnitPrice = 0;
        FilteredProducts.Clear();
    }

    // ── Carrito ────────────────────────────────────────────────────────────
    [RelayCommand]
    private void AddToCart()
    {
        ErrorMessage = "";

        if (SelectedProduct is null)
        {
            ErrorMessage = "Seleccioná un producto de la lista.";
            return;
        }
        if (Quantity <= 0)
        {
            ErrorMessage = "La cantidad debe ser mayor a cero.";
            return;
        }
        if (Quantity > SelectedProduct.Stock)
        {
            ErrorMessage = $"Stock insuficiente. Máximo disponible: {SelectedProduct.Stock}.";
            return;
        }

        UnitPrice = SelectedProduct.SalePrice;  

        var existing = CartItems.FirstOrDefault(i => i.ProductId == SelectedProduct.ProductId);
        if (existing is not null)
        {
            int newQty = existing.Quantity + Quantity;
            if (newQty > SelectedProduct.Stock)
            {
                ErrorMessage = $"El total acumulado supera el stock disponible ({SelectedProduct.Stock}).";
                return;
            }
            existing.Quantity = newQty;
        }
        else
        {
            CartItems.Add(new CartItemViewModel
            {
                ProductId = SelectedProduct.ProductId,
                ProductName = SelectedProduct.ProductName,
                Quantity = Quantity,
                UnitPrice = UnitPrice
            });
        }

        RecalculateTotal();
        SelectedProduct = null;
        ProductSearch = "";
        Quantity = 1;
        UnitPrice = 0;
    }

    [RelayCommand]
    private void RemoveFromCart(CartItemViewModel item)
    {
        CartItems.Remove(item);
        RecalculateTotal();
    }

    private void RecalculateTotal()
        => TotalAmount = CartItems.Sum(i => i.Quantity * i.UnitPrice);

    // Registrar venta 
    [RelayCommand]
    private async Task RegisterSaleAsync()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (SelectedCustomer is null)
        {
            ErrorMessage = "Selecciona un cliente para la venta.";
            return;
        }
        if (!CartItems.Any())
        {
            ErrorMessage = "Agrega al menos un producto al carrito.";
            return;
        }

        if (CartItems.Any() && CartItems.First().UnitPrice <= 0)
        {
            ErrorMessage = "Por favor, ingrese un precio de venta válido y mayor a 0.";
            return;
        }

        IsLoading = true;
        try
        {
            var dto = new SaleCreateDto
            {
                CustomerId = SelectedCustomer.Id,
                EmployeeId = SessionService.CurrentSession!.EmployeeId,
                SaleStatus = "REALIZADA",
                Items = CartItems
                    .Select(i => new SaleDetailItemDto(i.ProductId, i.Quantity, i.UnitPrice))
                    .ToList()
            };

            await _saleService.CreateNewSale(dto);

            SuccessMessage = "¡Venta registrada exitosamente!";
            ClearForm();

            // Recargar stock actualizado tras la venta
            var products = await _productService.GetAllProducts();
            _allProducts = products.ToList();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Helpers
    private void ClearForm()
    {
        SelectedCustomer = null;
        SelectedProduct = null;
        ProductSearch = "";
        Quantity = 1;
        UnitPrice = 0;
        TotalAmount = 0;
        CartItems.Clear();
        FilteredProducts.Clear();
    }
}

// CartItemViewModel 
// Clase propia para el carrito 
public partial class CartItemViewModel : ObservableObject
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";

    [ObservableProperty] private int _quantity;
    [ObservableProperty] private decimal _unitPrice;

    public decimal Subtotal => Quantity * UnitPrice;

    partial void OnQuantityChanged(int value) => OnPropertyChanged(nameof(Subtotal));
    partial void OnUnitPriceChanged(decimal value) => OnPropertyChanged(nameof(Subtotal));
}