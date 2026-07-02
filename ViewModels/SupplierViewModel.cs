using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Models;
using SistemaDistribuidora.Services;
using SistemaDistribuidora.Services.Interfaces;
using SistemaDistribuidora.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDistribuidora.ViewModels;

public partial class SupplierViewModel : ObservableObject
{
    private readonly INavigationService _nav;
    private readonly ISupplierService _supplierService;
    private List<SupplierListDto> _allSuppliers = new();

    public enum PersonType
    {
        Natural,
        Juridica
    }
    public IEnumerable<PersonType> PersonTypeOptions => Enum.GetValues<PersonType>();

    [ObservableProperty]
    private PersonType _selectedPersonType;
    public string WelcomeMessage => $"Hola, {SessionService.CurrentSession?.Username ?? "Usuario"}";
    public string UserRole => SessionService.CurrentSession?.Role ?? "";
    public bool IsAdmin => SessionService.IsAdmin;

    public SupplierViewModel(INavigationService navegationService, ISupplierService supplierService)
    {
        _nav = navegationService;
        _supplierService = supplierService;
    }

    [ObservableProperty] private bool _showWelcome = true;
    [ObservableProperty] private bool _showCrearProveedor;
    [ObservableProperty] private bool _showActualizarProveedor;
    [ObservableProperty] private bool _showListaProveedores;
    [ObservableProperty] private bool _showEliminarProveedor;
    [ObservableProperty] private bool _showReportes;

    [ObservableProperty] private ObservableCollection<SupplierDetailDto> _supplierDetail = new();

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private string _successMessage = "";

    [ObservableProperty] private string _createFullName = "";
    [ObservableProperty] private string _createIdentity = "";
    [ObservableProperty] private string _createAddress = "";
    [ObservableProperty] private string _createPhone = "";
    [ObservableProperty] private string _createPersonType = "Natural";

    [ObservableProperty] private string _updateFullName = "";
    [ObservableProperty] private string _updateIdentity = "";
    [ObservableProperty] private string _updateAddress = "";
    [ObservableProperty] private string _updatePhone = "";
    [ObservableProperty] private string _updatePersonType = "Natural";

    [ObservableProperty] private string _updateSearch = "";
    [ObservableProperty] private ObservableCollection<SupplierListDto> _filteredSupplierForUpdate = new();
    [ObservableProperty] private SupplierListDto? _selectedSupplierForUpdate;

    [ObservableProperty] private string _deleteSearch = "";
    [ObservableProperty] private ObservableCollection<SupplierListDto> _filteredSupplierForDelete = new();
    [ObservableProperty] private SupplierListDto? _selectedSupplierForDelete;

    [RelayCommand]
    public void GoToMainMenu() => _nav.NavigateTo<MainMenuView>();

    private void SetPanel(Action<SupplierViewModel> activar)
    {
        ShowWelcome = false;
        ShowCrearProveedor = false;
        ShowActualizarProveedor = false;
        ShowListaProveedores = false;
        ShowEliminarProveedor = false;
        ShowReportes = false;
        ErrorMessage = "";
        SuccessMessage = "";

        activar(this);
    }

    [RelayCommand]
    private void ShowCreateSupplierPanel()
    {
        ClearCreateForm();
        SetPanel(vm => vm.ShowCrearProveedor = true);
    }

    [RelayCommand]
    private async Task ShowUpdateSupplierPanel()
    {
        ClearUpdateForm();
        SelectedSupplierForUpdate = null;
        UpdateSearch = "";
        _allSuppliers.Clear();
        FilteredSupplierForUpdate.Clear();
        SetPanel(vm => vm.ShowActualizarProveedor = true);
        await LoadAllSuppliersAsync();
    }

    [RelayCommand]
    private async Task ShowSupplierListPanel()
    {
        SetPanel(vm => vm.ShowListaProveedores = true);
        IsLoading = true;
        ErrorMessage = "";
        SuccessMessage = "";
        try
        {
            var result = await _supplierService.GetSuppliersDetail();
            SupplierDetail.Clear();
            foreach (var r in result) SupplierDetail.Add(r);
        }
        catch
        {
            ErrorMessage = "Error al cargar los proveedores.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ShowDeleteSupplierPanel()
    {
        SelectedSupplierForDelete = null;
        DeleteSearch = "";
        _allSuppliers.Clear();
        FilteredSupplierForDelete.Clear();
        SetPanel(vm => vm.ShowEliminarProveedor = true);
        await LoadAllSuppliersAsync();
    }

    [RelayCommand]
    private void ShowReportsPanel() => SetPanel(vm => vm.ShowReportes = true);

    [RelayCommand]
    private async Task SaveSupplierAsync()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (string.IsNullOrWhiteSpace(CreateFullName) || string.IsNullOrWhiteSpace(CreateIdentity))
        {
            ErrorMessage = "Nombre y cédula son obligatorios.";
            return;
        }

        if (string.IsNullOrEmpty(SelectedPersonType.ToString()))
        {
            ErrorMessage = "Tipo de persona es obligatorio.";
            return;
        }

        IsLoading = true;
        try
        {
            var dto = new SupplierCreateDto
            {
                FullName = CreateFullName,
                Identity = CreateIdentity,
                Address = CreateAddress,
                Phone = CreatePhone,
                PersonType = SelectedPersonType.ToString()
            };
            await _supplierService.CreateNewSupplier(dto);
            SuccessMessage = "Proveedor creado exitosamente.";
            ClearCreateForm();
        }
        catch 
        {
            ErrorMessage = $"Error al crear";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadAllSuppliersAsync()
    {
        try
        {
            var result = await _supplierService.GetAllSuppliers();
            _allSuppliers = result.ToList();
        }
        catch
        {
            ErrorMessage = "Error al cargar los proveedores.";
        }
    }

    partial void OnUpdateSearchChanged(string value)
    {
        FilteredSupplierForUpdate.Clear();
        if (string.IsNullOrWhiteSpace(value) || _allSuppliers == null || !_allSuppliers.Any()) return;

        var matches = _allSuppliers
            .Where(s => s.FullName != null && s.FullName.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Take(6);

        foreach (var s in matches)
            FilteredSupplierForUpdate.Add(s);
    }

    [RelayCommand]
    private void SelectSupplierToUpdate(SupplierListDto supplier)
    {
        SelectedSupplierForUpdate = supplier;
        UpdateSearch = supplier.FullName;
        FilteredSupplierForUpdate.Clear();

        UpdateFullName = supplier.FullName;
        UpdateIdentity = supplier.IdentityCard;
        UpdatePhone = supplier.Phone;
        UpdateAddress = supplier.Address;

        if (Enum.TryParse<PersonType>(supplier.TypeOfPerson, true, out var parsedType))
        {
            SelectedPersonType = parsedType;
        }
    }

    [RelayCommand]
    private async Task UpdateSupplierAsync()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (SelectedSupplierForUpdate is null)
        {
            ErrorMessage = "Selecciona un proveedor de la lista.";
            return;
        }

        if (string.IsNullOrWhiteSpace(UpdateFullName) || string.IsNullOrWhiteSpace(UpdateIdentity))
        {
            ErrorMessage = "Nombre y cédula son obligatorios.";
            return;
        }

        IsLoading = true;
        try
        {
            var dto = new SupplierUpdateDto
            {
                SupplierId = SelectedSupplierForUpdate.Id,
                IdPerson = SelectedSupplierForUpdate.Id,
                FullName = UpdateFullName,
                Identity = UpdateIdentity,
                Address = UpdateAddress, 
                Phone = UpdatePhone,
                PersonType = SelectedPersonType.ToString()
            };

            await _supplierService.UpdateSupplier(dto);
            ClearUpdateForm();
            SelectedSupplierForUpdate = null;
            UpdateSearch = "";
            FilteredSupplierForUpdate.Clear();
            SuccessMessage = "Actualizado correctamente";

            await LoadAllSuppliersAsync();
        }
        catch 
        {
            ErrorMessage = $"Error al actualizar";
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnDeleteSearchChanged(string value)
    {
        FilteredSupplierForDelete.Clear();
        if (string.IsNullOrWhiteSpace(value) || _allSuppliers == null || !_allSuppliers.Any()) return;

        var matches = _allSuppliers
            .Where(s => s.FullName != null && s.FullName.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Take(6);

        foreach (var s in matches)
            FilteredSupplierForDelete.Add(s);
    }

    [RelayCommand]
    private void SelectSupplierToDelete(SupplierListDto supplier)
    {
        SelectedSupplierForDelete = supplier;
        DeleteSearch = supplier.FullName;
        FilteredSupplierForDelete.Clear();
    }

    [RelayCommand]
    private async Task DeleteSupplierAsync()
    {
        // Prximamente...
    }

    private void ClearCreateForm()
    {
        CreateFullName = "";
        CreateIdentity = "";
        CreateAddress = "";
        CreatePhone = "";
        CreatePersonType = "Natural";
    }

    private void ClearUpdateForm()
    {
        UpdateFullName = "";
        UpdateIdentity = "";
        UpdateAddress = "";
        UpdatePhone = "";
        UpdatePersonType = "Natural";
    }
}