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

public partial class CategoryViewModel : ObservableObject
{
    private readonly INavigationService _nav;
    private readonly ICategoryService _categoryService;

    private List<CategoryListDto> _allCategories = new();

    public string WelcomeMessage => $"Hola, {SessionService.CurrentSession?.Username ?? "Usuario"}";
    public string UserRole => SessionService.CurrentSession?.Role ?? "Empleado";
    public bool IsAdmin => SessionService.IsAdmin;

    public CategoryViewModel(INavigationService navigationService, ICategoryService categoryService)
    {
        _nav = navigationService;
        _categoryService = categoryService;
    }

    // PANELES DE NAVEGACION REACTIVA
    [ObservableProperty] private bool _showWelcome = true;
    [ObservableProperty] private bool _showCreateCategory;
    [ObservableProperty] private bool _showUpdateCategory;
    [ObservableProperty] private bool _showCategoryList;
    [ObservableProperty] private bool _showDeleteCategory;

    // ESTADOS GLOBALES
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private string _successMessage = "";

    // PROPIEDADES NUEVA CATEGORIA
    [ObservableProperty] private string _createCategoryName = "";

    // PROPIEDADES ACTUALIZAR CATEGORIA
    [ObservableProperty] private string _updateCategoryName = "";
    [ObservableProperty] private string _updateSearch = "";
    [ObservableProperty] private ObservableCollection<CategoryListDto> _filteredCategoriesForUpdate = new();
    [ObservableProperty] private CategoryListDto? _selectedCategoryForUpdate;

    // PROPIEDADES ELIMINAR CATEGORIA
    [ObservableProperty] private string _deleteSearch = "";
    [ObservableProperty] private ObservableCollection<CategoryListDto> _filteredCategoriesForDelete = new();
    [ObservableProperty] private CategoryListDto? _selectedCategoryForDelete;

    // CONSULTA GENERAL 
    [ObservableProperty] private ObservableCollection<CategoryDetailDto> _categoryDetail = new();
    [ObservableProperty] private ObservableCollection<CategoryListDto> _categoryList = new();

    [RelayCommand]
    public void GoToMainMenu() => _nav.NavigateTo<MainMenuView>();

    private void SetPanel(Action<CategoryViewModel> active)
    {
        ShowWelcome = false;
        ShowCreateCategory = false;
        ShowUpdateCategory = false;
        ShowCategoryList = false;
        ShowDeleteCategory = false;
        ErrorMessage = "";
        SuccessMessage = "";

        active(this);
    }

    [RelayCommand]
    private void ShowCreateCategoryPanel()
    {
        CreateCategoryName = "";
        SetPanel(vm => vm.ShowCreateCategory = true);
    }

    [RelayCommand]
    private async Task ShowUpdateCategoryPanel()
    {
        UpdateCategoryName = "";
        SelectedCategoryForUpdate = null;
        UpdateSearch = "";
        _allCategories.Clear();
        FilteredCategoriesForUpdate.Clear();
        SetPanel(vm => vm.ShowUpdateCategory = true);
        await LoadAllCategoriesAsync();
    }

    [RelayCommand]
    private async Task ShowCategoryListPanel()
    {
        SetPanel(vm => vm.ShowCategoryList = true);
        IsLoading = true;
        ErrorMessage = "";
        SuccessMessage = "";
        try
        {
            var result = await _categoryService.GetAllCategories();
            CategoryList.Clear();
            foreach (var c in result) CategoryList.Add(c);
        }
        catch
        {
            ErrorMessage = "Error al cargar el detalle de las categorías.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ShowDeleteCategoryPanel()
    { 
        DeleteSearch = "";
        _allCategories.Clear();
        FilteredCategoriesForDelete.Clear();
        SetPanel(vm => vm.ShowDeleteCategory = true);
        await LoadAllCategoriesAsync();
    }

    [RelayCommand]
    private async Task SaveCategoryAsync()
    {
       
        ErrorMessage = "";
        SuccessMessage = "";

        if (string.IsNullOrWhiteSpace(CreateCategoryName))
        {
            ErrorMessage = "El nombre de la categoria es obligatorio.";
            return;
        }

        IsLoading = true;
        try
        {
            var dto = new CategoryCreateDto { CategoryName = CreateCategoryName };
            await _categoryService.CreateNewCategory(dto);
            SuccessMessage = "Categoria creada exitosamente.";
            CreateCategoryName = "";
        }
        catch
        {
            ErrorMessage = "Error al guardar la categoria.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadAllCategoriesAsync()
    {
        try
        {
            var result = await _categoryService.GetAllCategories();
            _allCategories = result.ToList();
        }
        catch
        {
            ErrorMessage = "Error al inicializar catálogo de categorias.";
        }
    }

    // LOGICA FILTRADO ACTUALIZAR
    partial void OnUpdateSearchChanged(string value)
    {
        FilteredCategoriesForUpdate.Clear();
        if (string.IsNullOrWhiteSpace(value) || _allCategories == null || !_allCategories.Any()) return;

        var matches = _allCategories
            .Where(c => c.CategoryName != null && c.CategoryName.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Take(6);

        foreach (var c in matches)
            FilteredCategoriesForUpdate.Add(c);
    }

    [RelayCommand]
    private void SelectCategoryToUpdate(CategoryListDto category)
    {
        SelectedCategoryForUpdate = category;
        UpdateSearch = category.CategoryName;
        FilteredCategoriesForUpdate.Clear();
        UpdateCategoryName = category.CategoryName;
    }

    [RelayCommand]
    private async Task UpdateCategoryAsync()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (SelectedCategoryForUpdate?.CategoryId is null or 0)
        {
            ErrorMessage = "Selecciona una categoría válida de la lista.";
            return;
        }

        if (string.IsNullOrWhiteSpace(UpdateCategoryName))
        {
            ErrorMessage = "El nombre no puede quedar vacío.";
            return;
        }

        IsLoading = true;
        try
        {
            var dto = new CategoryUpdateDto { CategoryId = SelectedCategoryForUpdate.Value.CategoryId, CategoryName = UpdateCategoryName };
            await _categoryService.UpdateCategory(dto);

            UpdateCategoryName = "";
            SelectedCategoryForUpdate = null;
            UpdateSearch = "";
            FilteredCategoriesForUpdate.Clear();
            SuccessMessage = "Categoría actualizada correctamente.";

            await LoadAllCategoriesAsync();
        }
        catch
        {
            ErrorMessage = "Error al actualizar la categoría.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // LOGICA FILTRADO ELIMINAR
    partial void OnDeleteSearchChanged(string value)
    {
        FilteredCategoriesForDelete.Clear();
        if (string.IsNullOrWhiteSpace(value) || _allCategories == null || !_allCategories.Any()) return;

        var matches = _allCategories
            .Where(c => c.CategoryName != null && c.CategoryName.Contains(value, StringComparison.OrdinalIgnoreCase))
            .Take(6);

        foreach (var c in matches)
            FilteredCategoriesForDelete.Add(c);
    }

    [RelayCommand]
    private void SelectCategoryToDelete(CategoryListDto category)
    {
        SelectedCategoryForDelete = category;
        DeleteSearch = category.CategoryName;
        FilteredCategoriesForDelete.Clear();
    }

    [RelayCommand]
    private async Task DeleteCategoryAsync()
    {
        

        IsLoading = true;
        ErrorMessage = "";
        SuccessMessage = "";
        try
        {
            await _categoryService.DeleteCategory(SelectedCategoryForDelete!.Value.CategoryId);

            SelectedCategoryForDelete = null;
            DeleteSearch = "";
            FilteredCategoriesForDelete.Clear();
            SuccessMessage = "Categoría eliminada con éxito.";
            await LoadAllCategoriesAsync();
        }
        catch
        {
            ErrorMessage = $"Error al dar de baja la categoría";
        }
        finally
        {
            IsLoading = false;
        }
    }
}