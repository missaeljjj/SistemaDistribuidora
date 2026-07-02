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

public partial class ProductViewModel : ObservableObject
{
    private readonly INavigationService _nav;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly ISupplierService _supplierService;

    private List<InventoryDetailDto> _allProducts = new();

    public string WelcomeMessage => $"Hola, {SessionService.CurrentSession?.Username ?? "Usuario"}";
    public string UserRole => SessionService.CurrentSession?.Role ?? "";
    public bool IsAdmin => SessionService.IsAdmin;

    public ProductViewModel(
        INavigationService navigationService,
        IProductService productService,
        ICategoryService categoryService,
        ISupplierService supplierService)
    {
        _nav = navigationService;
        _productService = productService;
        _categoryService = categoryService;
        _supplierService = supplierService;
    }

    // Listas para los ComboBoxes
    [ObservableProperty] private ObservableCollection<CategoryListDto> _categories = new();
    [ObservableProperty] private ObservableCollection<SupplierListDto> _suppliers = new();

    [ObservableProperty] private CategoryListDto? _selectedCategory;
    [ObservableProperty] private SupplierListDto? _selectedSupplier;

    // Paneles lógicos de la vista
    [ObservableProperty] private bool _showWelcome = true;
    [ObservableProperty] private bool _showCreateProduct = false;
    [ObservableProperty] private bool _showUpdateProductPrice = false; // Corregido minúscula inicial para consistencia
    [ObservableProperty] private bool _showUpdateProduct = false;
    [ObservableProperty] private bool _showProductList = false;
    [ObservableProperty] private bool _showDeleteProduct = false;
    [ObservableProperty] private bool _showReports = false;

    [ObservableProperty] private ObservableCollection<InventoryDetailDto> _productList = new();

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private string _successMessage = "";

    // Campos del formulario: Crear Producto
    [ObservableProperty] private string _createProductName = "";

    // Campos del formulario: Actualizar Producto Completo
    [ObservableProperty] private string _updateProductName = "";
    [ObservableProperty] private decimal? _updateSalePrice = 0;

    // Campo del formulario: Actualizar SOLO precio (Procedimiento Almacenado)
    [ObservableProperty] private decimal? _updateOnlySalePrice;

    [ObservableProperty] private string _updateSearch = "";
    [ObservableProperty] private ObservableCollection<InventoryDetailDto> _filteredProductsForUpdate = new();
    [ObservableProperty] private InventoryDetailDto? _selectedProductForUpdate;

    [ObservableProperty] private string _deleteSearch = "";
    [ObservableProperty] private ObservableCollection<InventoryDetailDto> _filteredProductsForDelete = new();
    [ObservableProperty] private InventoryDetailDto? _selectedProductForDelete;

    [RelayCommand]
    public void GoToMainMenu() => _nav.NavigateTo<MainMenuView>();

    private void SetPanel(Action<ProductViewModel> activar)
    {
        ShowWelcome = false;
        ShowCreateProduct = false;
        ShowUpdateProduct = false;
        ShowUpdateProductPrice = false; // Agregado para que se apague al cambiar de menú
        ShowProductList = false;
        ShowDeleteProduct = false;
        ShowReports = false;
        ErrorMessage = "";
        SuccessMessage = "";

        activar(this);
    }

    private async Task LoadCombosDataAsync()
    {
        try
        {
            var categories = await _categoryService.GetAllCategories();
            Categories.Clear();
            foreach (var c in categories) Categories.Add(c);

            var supliers = await _supplierService.GetAllSuppliers();
            Suppliers.Clear();
            foreach (var s in supliers) Suppliers.Add(s);
        }
        catch
        {
            ErrorMessage = "Error al cargar los catálogos de categorías o proveedores.";
        }
    }

    [RelayCommand]
    private async Task ShowCreateProductPanel()
    {
        ClearCreateForm();
        SetPanel(vm => vm.ShowCreateProduct = true);
        await LoadCombosDataAsync();
    }

    [RelayCommand]
    private async Task ShowUpdateProductPanel()
    {
        ClearUpdateForm();
        SelectedProductForUpdate = null;
        UpdateSearch = "";
        _allProducts.Clear();
        FilteredProductsForUpdate.Clear();
        SetPanel(vm => vm.ShowUpdateProduct = true);
        await LoadCombosDataAsync();
        await LoadAllProductsAsync();
    }

    // NUEVO COMANDO: Activa el panel exclusivo del SP para actualizar el precio
    [RelayCommand]
    private async Task ShowUpdateProductPricePanel()
    {
        ClearUpdateForm();
        SelectedProductForUpdate = null;
        UpdateSearch = "";
        _allProducts.Clear();
        FilteredProductsForUpdate.Clear();
        SetPanel(vm => vm.ShowUpdateProductPrice = true);
        await LoadAllProductsAsync();
    }

    [RelayCommand]
    private async Task ShowProductListPanel()
    {
        SetPanel(vm => vm.ShowProductList = true);
        IsLoading = true;
        ErrorMessage = "";
        SuccessMessage = "";
        try
        {
            var result = await _productService.GetAllProducts();
            ProductList.Clear();
            foreach (var c in result) ProductList.Add(c);
        }
        catch
        {
            ErrorMessage = "Error al cargar la lista de productos.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ShowDeleteProductPanel()
    {
        SelectedProductForDelete = null;
        DeleteSearch = "";
        _allProducts.Clear();
        FilteredProductsForDelete.Clear();
        SetPanel(vm => vm.ShowDeleteProduct = true);
        await LoadAllProductsAsync();
    }

    [RelayCommand]
    private void ShowReportsPanel() => SetPanel(vm => vm.ShowReports = true);

    [RelayCommand]
    private async Task SaveProductAsync()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (string.IsNullOrWhiteSpace(CreateProductName))
        {
            ErrorMessage = "El nombre del producto es obligatorio.";
            return;
        }

        if (SelectedCategory is null)
        {
            ErrorMessage = "Debes seleccionar una categoría.";
            return;
        }

        if (SelectedSupplier is null)
        {
            ErrorMessage = "Debes seleccionar un proveedor.";
            return;
        }

        IsLoading = true;
        try
        {
            var dto = new ProductCreateDto
            {
                Name = CreateProductName,
                CategoryId = SelectedCategory.Value.CategoryId,
                SupplierId = SelectedSupplier.Id
            };

            await _productService.CreateNewProduct(dto);
            SuccessMessage = "Producto creado exitosamente.";
            ClearCreateForm();
        }
        catch
        {
            ErrorMessage = $"Error al crear el producto: ";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadAllProductsAsync()
    {
        try
        {
            var result = await _productService.GetAllProducts();
            _allProducts = result.ToList();
        }
        catch
        {
            ErrorMessage = "Error al cargar la lista de productos.";
        }
    }

    partial void OnUpdateSearchChanged(string value)
    {
        FilteredProductsForUpdate.Clear();
        if (string.IsNullOrWhiteSpace(value) || _allProducts == null || !_allProducts.Any()) return;

        var matches = _allProducts
            .Where(p => p.ProductName != null && p.ProductName.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Take(6);

        foreach (var p in matches)
            FilteredProductsForUpdate.Add(p);
    }

    [RelayCommand]
    private void SelectProductToUpdate(InventoryDetailDto product)
    {
        SelectedProductForUpdate = product;
        UpdateSearch = product.ProductName;
        FilteredProductsForUpdate.Clear();

        UpdateProductName = product.ProductName;
        UpdateOnlySalePrice = product.SalePrice; 

        if (Categories.Any() && Suppliers.Any())
        {
            SelectedCategory = Categories.FirstOrDefault(c => c.CategoryName == product.CategoryName);
            SelectedSupplier = Suppliers.FirstOrDefault(s => s.FullName == product.SupplierName);
        }
    }

    [RelayCommand]
    private async Task UpdateProductAsync()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (SelectedProductForUpdate is null)
        {
            ErrorMessage = "Selecciona un producto de la lista.";
            return;
        }

        if (string.IsNullOrWhiteSpace(UpdateProductName))
        {
            ErrorMessage = "El nombre de producto es obligatorio.";
            return;
        }

        if (SelectedCategory is null || SelectedSupplier is null)
        {
            ErrorMessage = "La categoría y el proveedor son obligatorios.";
            return;
        }

        IsLoading = true;
        try
        {
            var dto = new ProductUpdateDto
            {
                ProductId = SelectedProductForUpdate.ProductId,
                Name = UpdateProductName,
                SupplierId = SelectedSupplier.Id,
                CategoryId = SelectedCategory.Value.CategoryId
            };

            await _productService.UpdateProduct(dto);

            SelectedProductForUpdate = null;
            UpdateSearch = "";
            FilteredProductsForUpdate.Clear();
            SuccessMessage = "Producto actualizado correctamente.";

            await LoadAllProductsAsync();

            var result = await _productService.GetAllProducts();
            ProductList.Clear();
            foreach (var p in result) ProductList.Add(p);
        }
        catch
        {
            ErrorMessage = $"Error al actualizar el producto";
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnDeleteSearchChanged(string value)
    {
        FilteredProductsForDelete.Clear();
        if (string.IsNullOrWhiteSpace(value) || _allProducts == null || !_allProducts.Any()) return;

        var matches = _allProducts
            .Where(p => p.ProductName != null && p.ProductName.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Take(6);

        foreach (var p in matches)
            FilteredProductsForDelete.Add(p);
    }


    [RelayCommand]
    private async Task UpdateProductPriceAsync()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (SelectedProductForUpdate is null)
        {
            ErrorMessage = "Selecciona un producto de la lista.";
            return;
        }

        if (UpdateOnlySalePrice is null || UpdateOnlySalePrice <= 0)
        {
            ErrorMessage = "Por favor, ingrese un precio de venta válido y mayor a 0.";
            return;
        }

        IsLoading = true;
        try
        {
            bool isUpdated = await _productService.UpdateProductPrice(
                SelectedProductForUpdate.ProductId,
                UpdateOnlySalePrice.Value
            );

            if (isUpdated)
            {
                SuccessMessage = "Precio de venta actualizado exitosamente";
                UpdateOnlySalePrice = null;
                SelectedProductForUpdate = null;
                UpdateSearch = "";

                await LoadAllProductsAsync();

                var result = await _productService.GetAllProducts();
                ProductList.Clear();
                foreach (var p in result) ProductList.Add(p);
            }
        }
        catch 
        {
            ErrorMessage = $"Error al cambiar el precio";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ClearCreateForm()
    {
        CreateProductName = "";
        SelectedCategory = null;
        SelectedSupplier = null;
    }

    private void ClearUpdateForm()
    {
        UpdateProductName = "";
        UpdateSalePrice = 0;
        UpdateOnlySalePrice = null;
        SelectedSupplier = null;
        SelectedCategory = null;
    }
}