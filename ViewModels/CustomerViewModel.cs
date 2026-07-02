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

public partial class CustomerViewModel : ObservableObject
{
    private readonly INavigationService _nav;
    private readonly ICustomerService _customerService;
    private List<CustomerListDto> _allCustomers = new();

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

    public CustomerViewModel(INavigationService navegationService, ICustomerService customerService)
    {
        _nav = navegationService;
        _customerService = customerService;
    }

    [ObservableProperty] private bool _showWelcome = true;
    [ObservableProperty] private bool _showCrearCliente;
    [ObservableProperty] private bool _showActualizarCliente;
    [ObservableProperty] private bool _showListaClientes;
    [ObservableProperty] private bool _showEliminarCliente;
    [ObservableProperty] private bool _showReportes;

    [ObservableProperty] private ObservableCollection<CustomerListDto> _customerList = new();

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private string _successMessage = "";

    [ObservableProperty] private string _createFullName = "";
    [ObservableProperty] private string _createIdentity = "";
    [ObservableProperty] private string _createAddress = "";
    [ObservableProperty] private string _createPhone = "";
    [ObservableProperty] private string _createPersonType = "";

    [ObservableProperty] private string _updateFullName = "";
    [ObservableProperty] private string _updateIdentity = "";
    [ObservableProperty] private string _updateAddress = "";
    [ObservableProperty] private string _updatePhone = "";
    [ObservableProperty] private string _updatePersonType = "";

    [ObservableProperty] private string _updateSearch = "";
    [ObservableProperty] private ObservableCollection<CustomerListDto> _filteredCustomersForUpdate = new();
    [ObservableProperty] private CustomerListDto? _selectedCustomerForUpdate;

    [ObservableProperty] private string _deleteSearch = "";
    [ObservableProperty] private ObservableCollection<CustomerListDto> _filteredCustomersForDelete = new();
    [ObservableProperty] private CustomerListDto? _selectedCustomerForDelete;

    [RelayCommand]
    public void GoToMainMenu() => _nav.NavigateTo<MainMenuView>();

    private void SetPanel(Action<CustomerViewModel> activar)
    {
        ShowWelcome = false;
        ShowCrearCliente = false;
        ShowActualizarCliente = false;
        ShowListaClientes = false;
        ShowEliminarCliente = false;
        ShowReportes = false;
        ErrorMessage = "";
        SuccessMessage = "";

        activar(this);
    }

    [RelayCommand]
    private void ShowCreateCustomerPanel()
    {
        ClearCreateForm();
        SetPanel(vm => vm.ShowCrearCliente = true);
    }

    [RelayCommand]
    private async Task ShowUpdateCustomerPanel()
    {
        ClearUpdateForm();
        SelectedCustomerForUpdate = null;
        UpdateSearch = "";
        _allCustomers.Clear();
        FilteredCustomersForUpdate.Clear();
        SetPanel(vm => vm.ShowActualizarCliente = true);
        await LoadAllCustomersAsync();
    }

    [RelayCommand]
    private async Task ShowCustomerListPanel()
    {
        SetPanel(vm => vm.ShowListaClientes = true);
        IsLoading = true;
        ErrorMessage = "";
        SuccessMessage = "";
        try
        {
            var result = await _customerService.GetAllCustomers();
            CustomerList.Clear();
            foreach (var c in result) CustomerList.Add(c);
        }
        catch
        {
            ErrorMessage = "Error al cargar los clientes.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ShowDeleteCustomerPanel()
    {
        SelectedCustomerForDelete = null;
        DeleteSearch = "";
        _allCustomers.Clear();
        FilteredCustomersForDelete.Clear();
        SetPanel(vm => vm.ShowEliminarCliente = true);
        await LoadAllCustomersAsync();
    }

    [RelayCommand]
    private void ShowReportsPanel() => SetPanel(vm => vm.ShowReportes = true);

    [RelayCommand]
    private async Task SaveCustomerAsync()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (string.IsNullOrWhiteSpace(CreateFullName) || string.IsNullOrWhiteSpace(CreateIdentity))
        {
            ErrorMessage = "Nombre y cedula son obligatorios.";
            return;
        }

        if(string.IsNullOrEmpty(SelectedPersonType.ToString()))
        {
            ErrorMessage = "Tipo de persona es obligatorio.";
            return;
        }

        IsLoading = true;
        try
        {
            var dto = new CustomerCreateDto
            {
                FullName = CreateFullName,
                Identity = CreateIdentity,
                Address = CreateAddress,
                Phone = CreatePhone,
                PersonType = SelectedPersonType.ToString()
            };
            await _customerService.CreateNewCustomer(dto);
            SuccessMessage = "Cliente creado exitosamente.";
            ClearCreateForm();
        }
        catch
        {
            ErrorMessage = $"Error al crear ";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadAllCustomersAsync()
    {
        try
        {
            var result = await _customerService.GetAllCustomers();
            _allCustomers = result.ToList();
        }
        catch
        {
            ErrorMessage = "Error al cargar los clientes.";
        }
    }

    partial void OnUpdateSearchChanged(string value)
    {
        FilteredCustomersForUpdate.Clear();
        if (string.IsNullOrWhiteSpace(value) || _allCustomers == null || !_allCustomers.Any()) return;

        var matches = _allCustomers
            .Where(c => c.FullName != null && c.FullName.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Take(6);

        foreach (var c in matches)
            FilteredCustomersForUpdate.Add(c);
    }

    [RelayCommand]
    private void SelectCustomerToUpdate(CustomerListDto customer)
    {
        SelectedCustomerForUpdate = customer;
        UpdateSearch = customer.FullName;
        FilteredCustomersForUpdate.Clear();

        UpdateFullName = customer.FullName;
        UpdateIdentity = customer.IdentityCard;
        UpdatePhone = customer.Phone;
        UpdateAddress = customer.Address;

        if (Enum.TryParse<PersonType>(customer.TypeOfPerson, true, out var parsedType))
        {
            SelectedPersonType = parsedType;
        }
    }

    [RelayCommand]
    private async Task UpdateCustomerAsync()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (SelectedCustomerForUpdate is null)
        {
            ErrorMessage = "Selecciona un cliente de la lista.";
            return;
        }

        if (string.IsNullOrWhiteSpace(UpdateFullName) || string.IsNullOrWhiteSpace(UpdateIdentity))
        {
            ErrorMessage = "Nombre y cedula son obligatorios.";
            return;
        }

        IsLoading = true;
        try
        {
            var dto = new CustomerUpdateDto
            {
                CustomerId = SelectedCustomerForUpdate.Id,
                IdPerson = SelectedCustomerForUpdate.Id,
                FullName = UpdateFullName,
                Identity = UpdateIdentity,
                Address = UpdateAddress,
                Phone = UpdatePhone,
                PersonType = SelectedPersonType.ToString()
            };

           
            ClearUpdateForm();
            
            await _customerService.UpdateCustomer(dto);
            SelectedCustomerForUpdate = null;
            UpdateSearch = "";
            FilteredCustomersForUpdate.Clear();
            SuccessMessage = "Actualizado correctamente";

            await LoadAllCustomersAsync();
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
        FilteredCustomersForDelete.Clear();
        if (string.IsNullOrWhiteSpace(value) || _allCustomers == null || !_allCustomers.Any()) return;

        var matches = _allCustomers
            .Where(c => c.FullName != null && c.FullName.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Take(6);

        foreach (var c in matches)
            FilteredCustomersForDelete.Add(c);
    }

    [RelayCommand]
    private void SelectCustomerToDelete(CustomerListDto customer)
    {
        SelectedCustomerForDelete = customer;
        DeleteSearch = customer.FullName;
        FilteredCustomersForDelete.Clear();
    }

    [RelayCommand]
    private async Task DeleteCustomerAsync()
    {
        if (SelectedCustomerForDelete is null) return;

        IsLoading = true;
        ErrorMessage = "";
        SuccessMessage = "";
        try
        {
            await _customerService.DeleteCustomer(SelectedCustomerForDelete.Id);

            SelectedCustomerForDelete = null;
            DeleteSearch = "";
            FilteredCustomersForDelete.Clear();

            SuccessMessage = "Cliente eliminado exitosamente.";
            await LoadAllCustomersAsync();
        }
        catch
        {
            ErrorMessage = "Error al eliminar el cliente.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ClearCreateForm()
    {
        CreateFullName = "";
        CreateIdentity = "";
        CreateAddress = "";
        CreatePhone = "";
        CreatePersonType = "";
    }

    private void ClearUpdateForm()
    {
        UpdateFullName = "";
        UpdateIdentity = "";
        UpdateAddress = "";
        UpdatePhone = "";
        UpdatePersonType = "";
    }
}