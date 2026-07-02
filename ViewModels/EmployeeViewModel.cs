using SistemaDistribuidora.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using SistemaDistribuidora.Services;
using CommunityToolkit.Mvvm.Input;
using SistemaDistribuidora.Views;
using System.Collections.Generic;
using SistemaDistribuidora.DTOs;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace SistemaDistribuidora.ViewModels;

public partial class EmployeeViewModel : ObservableObject
{
    private readonly INavigationService _nav;
    private readonly IEmployeeService _employeeService;
    private List<EmployeeListDto> _allEmployees = new();

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

    public EmployeeViewModel(INavigationService navegationService, IEmployeeService employeeservice)
    {
        _nav = navegationService;
        _employeeService = employeeservice;
    }

    [ObservableProperty] private bool _showWelcome = true;
    [ObservableProperty] private bool _showCreateEmployee;
    [ObservableProperty] private bool _showUpdateEmployee;
    [ObservableProperty] private bool _showEmployeeList;
    [ObservableProperty] private bool _showDeleteEmployee;
    [ObservableProperty] private bool _showReports;

    [ObservableProperty] private ObservableCollection<EmployeeDetailDto> _employeeDetail = new();

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private string _successMessage = "";

    [ObservableProperty] private string _createFullName = "";
    [ObservableProperty] private string _createIdentity = "";
    [ObservableProperty] private string _createAddress = "";
    [ObservableProperty] private string _createPhone = "";
    [ObservableProperty] private string _createPersonType = "Natural";
    [ObservableProperty] private string _createRole = "";

    [ObservableProperty] private string _updateFullName = "";
    [ObservableProperty] private string _updateIdentity = "";
    [ObservableProperty] private string _updateAddress = "";
    [ObservableProperty] private string _updatePhone = "";
    [ObservableProperty] private string _updatePersonType = "Natural";
    [ObservableProperty] private string _updateRole = "";

    [ObservableProperty] private string _updateSearch = "";
    [ObservableProperty] private ObservableCollection<EmployeeListDto> _filteredEmployeeForUpdate = new();
    [ObservableProperty] private EmployeeListDto? _selectedEmployeeForUpdate;

    [ObservableProperty] private string _deleteSearch = "";
    [ObservableProperty] private ObservableCollection<EmployeeListDto> _filteredEmployeesForDelete = new();
    [ObservableProperty] private EmployeeListDto? _selectedEmployeeForDelete;

    [RelayCommand]
    public void GoToMainMenu() => _nav.NavigateTo<MainMenuView>();

    private void SetPanel(Action<EmployeeViewModel> active)
    {
        ShowWelcome = false;
        ShowCreateEmployee = false;
        ShowUpdateEmployee = false;
        ShowEmployeeList = false;
        ShowDeleteEmployee = false;
        ShowReports = false;
        ErrorMessage = "";
        SuccessMessage = "";

        active(this);
    }

    [RelayCommand]
    private void ShowCreateEmployeePanel()
    {
        ClearCreateForm();
        SetPanel(vm => vm.ShowCreateEmployee = true);
    }

    [RelayCommand]
    private async Task ShowUpdateEmployeePanel()
    {
        ClearUpdateForm();
        SelectedEmployeeForUpdate = null;
        UpdateSearch = "";
        _allEmployees.Clear();
        FilteredEmployeeForUpdate.Clear();
        SetPanel(vm => vm.ShowUpdateEmployee = true);
        await LoadAllEmployeeAsync();
    }

    [RelayCommand]
    private async Task ShowEmployeeListPanel()
    {
        SetPanel(vm => vm.ShowEmployeeList = true);
        IsLoading = true;
        ErrorMessage = "";
        SuccessMessage = "";
        try
        {
            var result = await _employeeService.GetEmployeesDetail();
            EmployeeDetail.Clear();
            foreach (var r in result) EmployeeDetail.Add(r);
        }
        catch
        {
            ErrorMessage = "Error al cargar los empleados.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ShowDeleteEmployeePanel()
    {
        DeleteSearch = "";
        _allEmployees.Clear();
        FilteredEmployeesForDelete.Clear();
        SetPanel(vm => vm.ShowDeleteEmployee = true);
        await LoadAllEmployeeAsync();
    }

    [RelayCommand]
    private void ShowReportsPanel() => SetPanel(vm => vm.ShowReports = true);



    [RelayCommand]
    private async Task SaveEmployeeAsync()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (string.IsNullOrWhiteSpace(CreateFullName) || string.IsNullOrWhiteSpace(CreateIdentity))
        {
            ErrorMessage = "Nombre y cédula son obligatorios.";
            return;
        }

        IsLoading = true;
        try
        {
            var dto = new EmployeeCreateDto
            {
                FullName = CreateFullName,
                Identity = CreateIdentity,
                Address = CreateAddress,
                Phone = CreatePhone,
                PersonType = CreatePersonType,
                Position = CreateRole
            };
            await _employeeService.CreateNewEmployee(dto);
            SuccessMessage = "Empleado creado exitosamente.";
            ClearCreateForm();
        }
        catch 
        {
            ErrorMessage = $"Error al crear el empleado.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadAllEmployeeAsync()
    {
        try
        {
            var result = await _employeeService.GetAllEmployees();
            _allEmployees = result.ToList();
        }
        catch
        {
            ErrorMessage = "Error al cargar los empleados.";
        }
    }

    partial void OnUpdateSearchChanged(string value)
    {
        FilteredEmployeeForUpdate.Clear();
        if (string.IsNullOrWhiteSpace(value) || _allEmployees == null || !_allEmployees.Any()) return;

        var matches = _allEmployees
            .Where(e => e.FullName != null && e.FullName.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Take(6);

        foreach (var e in matches)
            FilteredEmployeeForUpdate.Add(e);
    }

    [RelayCommand]
    private void SelectEmployeeToUpdate(EmployeeListDto employee)
    {
        SelectedEmployeeForUpdate = employee;
        UpdateSearch = employee.FullName;
        FilteredEmployeeForUpdate.Clear();

        UpdateFullName = employee.FullName;
        UpdateIdentity = employee.IdentityCard;
        UpdatePhone = employee.Phone;
        UpdateAddress = employee.Address;
        UpdatePersonType = employee.TypeOfPerson ?? "Natural";
        UpdateRole = employee.Position;
    }

    [RelayCommand]
    private async Task UpdateEmployeeAsync()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (SelectedEmployeeForUpdate is null)
        {
            ErrorMessage = "Selecciona un empleado de la lista.";
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
            var dto = new EmployeeUpdateDto
            {
                EmployeeId = SelectedEmployeeForUpdate.Id,
                IdPerson = SelectedEmployeeForUpdate.Id,
                FullName = UpdateFullName,
                Identity = UpdateIdentity,
                Position = UpdateRole,
                Address = UpdateAddress,
                Phone = UpdatePhone,
                PersonType = UpdatePersonType
            };

            await _employeeService.UpdateEmployee(dto);
            ClearUpdateForm();
            SelectedEmployeeForUpdate = null;
            UpdateSearch = "";
            FilteredEmployeeForUpdate.Clear();
            SuccessMessage = "Actualizado correctamente";

            await LoadAllEmployeeAsync();
        }
        catch  
        {
            ErrorMessage = $"ERROR AL ACTUALIZAR EL EMPLEADO.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnDeleteSearchChanged(string value)
    {
        FilteredEmployeesForDelete.Clear();
        if (string.IsNullOrWhiteSpace(value) || _allEmployees == null || !_allEmployees.Any()) return;

        var matches = _allEmployees
            .Where(e => e.FullName != null && e.FullName.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Take(6);

        foreach (var e in matches)
            FilteredEmployeesForDelete.Add(e);
    }

    [RelayCommand]
    private void SelectEmployeeToDelete(EmployeeListDto Employee)
    {
        SelectedEmployeeForDelete = Employee;
        DeleteSearch = Employee.FullName;
        FilteredEmployeesForDelete.Clear();
    }

    [RelayCommand]
    private async Task DeleteEmployeeAsync()
    {
        if (SelectedEmployeeForDelete is null)
            return;

        IsLoading = true;
        ErrorMessage = "";
        SuccessMessage = "";
        try
        {
            await _employeeService.DeleteEmployee(SelectedEmployeeForDelete.Id);

            SelectedEmployeeForDelete = null;
            DeleteSearch = "";
            FilteredEmployeesForDelete.Clear();

            SuccessMessage = "Empleado eliminado exitosamente.";
            await LoadAllEmployeeAsync();
        }
        catch
        {
            ErrorMessage = "Error al eliminar el empleado.";
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