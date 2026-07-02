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

public partial class PurchaseViewModel : ObservableObject
{
    private readonly INavigationService _nav;
    private readonly IPurchaseService _purchaseService;
    private readonly IProductService _productService;
    private readonly ISupplierService _supplierService;

    private List<InventoryDetailDto> _allProducts = new();
    // Solo los productos del proveedor seleccionado
    private List<InventoryDetailDto> _supplierProducts = new();

    public string WelcomeMessage => $"Hola, {SessionService.CurrentSession?.Username ?? "Usuario"}";
    public string UserRole => SessionService.CurrentSession?.Role ?? "";

    public PurchaseViewModel(
        INavigationService nav,
        IPurchaseService purchaseService,
        IProductService productService,
        ISupplierService supplierService)
    {
        _nav = nav;
        _purchaseService = purchaseService;
        _productService = productService;
        _supplierService = supplierService;

        ShowWelcome = true;
        ShowRegisterPurchase = false;
    }

    [ObservableProperty] private bool _showWelcome;
    [ObservableProperty] private bool _showRegisterPurchase;

    [RelayCommand]
    public void ShowRegisterPurchasePanel()
    {
        ResetPanels();
        ShowRegisterPurchase = true;
    }

    private void ResetPanels()
    {
        ShowWelcome = false;
        ShowRegisterPurchase = false;
        ErrorMessage = "";
        SuccessMessage = "";
    }

    // Estado UI 
    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private string _successMessage = "";

    // Proveedores

    [ObservableProperty] private ObservableCollection<SupplierListDto> _suppliers = new();
    [ObservableProperty] private SupplierListDto? _selectedSupplier;

    partial void OnSelectedSupplierChanged(SupplierListDto? value)
    {
        // Al cambiar proveedor se filtran los productos relacionados a el
        SelectedProduct = null;
        ProductSearch = "";
        FilteredProducts.Clear();

        if (value is null)
        {
            _supplierProducts = new();
            return;
        }

        _supplierProducts = _allProducts
            .Where(p => p.SupplierName.Equals(value.FullName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    //Buscador de productos
    [ObservableProperty] private string _productSearch = "";
    [ObservableProperty] private ObservableCollection<InventoryDetailDto> _filteredProducts = new();
    [ObservableProperty] private InventoryDetailDto? _selectedProduct;
    [ObservableProperty] private int _quantity = 1;
    [ObservableProperty] private decimal _unitPrice = 0;

    public string StockInfo => SelectedProduct is not null
        ? $"Stock actual: {SelectedProduct.Stock}"
        : "";

//carito
    [ObservableProperty] private ObservableCollection<PurchaseCartItemViewModel> _cartItems = new();
    [ObservableProperty] private decimal _totalAmount = 0;


    [RelayCommand]
    public void GoToMainMenu() => _nav.NavigateTo<MainMenuView>();

 
    [RelayCommand]
    public async Task InitializeAsync()
    {
        IsLoading = true;
        ErrorMessage = "";
        try
        {
            var suppliers = await _supplierService.GetAllSuppliers();
            Suppliers.Clear();
            foreach (var s in suppliers) Suppliers.Add(s);

            var products = await _productService.GetAllProducts();
            _allProducts = products.ToList();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
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

        // Busca solo en los productos del proveedor seleccionado
        var source = SelectedSupplier is not null ? _supplierProducts : _allProducts;

        var matches = source
            .Where(p => p.ProductName.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Take(6);

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
        OnPropertyChanged(nameof(StockInfo));
    }

    // Carrito 
    [RelayCommand]
    private void AddToCart()
    {
        ErrorMessage = "";

        if (SelectedProduct is null)
        {
            ErrorMessage = "Selecciona un producto de la lista.";
            return;
        }
        if (Quantity <= 0)
        {
            ErrorMessage = "La cantidad debe ser mayor a cero.";
            return;
        }
        if (UnitPrice <= 0)
        {
            ErrorMessage = "Ingresa el precio de compra.";
            return;
        }

        var existing = CartItems.FirstOrDefault(i => i.ProductId == SelectedProduct.ProductId);
        if (existing is not null)
        {
            existing.Quantity += Quantity;
        }
        else
        {
            CartItems.Add(new PurchaseCartItemViewModel
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
        OnPropertyChanged(nameof(StockInfo));
    }

    [RelayCommand]
    private void RemoveFromCart(PurchaseCartItemViewModel item)
    {
        CartItems.Remove(item);
        RecalculateTotal();
    }

    private void RecalculateTotal()
        => TotalAmount = CartItems.Sum(i => i.Quantity * i.UnitPrice);

    // Registrar compra
    [RelayCommand]
    private async Task RegisterPurchaseAsync()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (SelectedSupplier is null)
        {
            ErrorMessage = "Selecciona un proveedor para la compra.";
            return;
        }
        if (!CartItems.Any())
        {
            ErrorMessage = "Agrega al menos un producto al carrito.";
            return;
        }

        IsLoading = true;
        try
        {
            var dto = new PurchaseCreateDto
            {
                SupplierId = SelectedSupplier.Id,
                EmployeeId = SessionService.CurrentSession!.EmployeeId,
                PurchaseStatus = "REALIZADA",
                Items = CartItems
                    .Select(i => new PurchaseDetailItemDto(i.ProductId, i.Quantity, i.UnitPrice))
                    .ToList()
            };

            await _purchaseService.CreateNewPurchase(dto);

            SuccessMessage = "Compra registrada exitosamente";
            ClearForm();

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
        SelectedSupplier = null;
        SelectedProduct = null;
        ProductSearch = "";
        Quantity = 1;
        UnitPrice = 0;
        TotalAmount = 0;
        CartItems.Clear();
        FilteredProducts.Clear();
        OnPropertyChanged(nameof(StockInfo));
    }
}

// PurchaseCartItemViewModel
public partial class PurchaseCartItemViewModel : ObservableObject
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";

    [ObservableProperty] private int _quantity;
    [ObservableProperty] private decimal _unitPrice;

    public decimal Subtotal => Quantity * UnitPrice;

    partial void OnQuantityChanged(int value) => OnPropertyChanged(nameof(Subtotal));
    partial void OnUnitPriceChanged(decimal value) => OnPropertyChanged(nameof(Subtotal));
}