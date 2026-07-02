using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Repositories.Interfaces;
using System.Threading.Tasks;
using SistemaDistribuidora.Mappers;
using System.Linq;

namespace SistemaDistribuidora.Services;

public class AppCache : IAppCache
{
    private readonly IMemoryCache _Cache;
    private readonly ICustomerRepository _CustomerRepository;
    private readonly IEmployeeRepository _EmployeeRepository;
    private readonly ICategoryRepository _CategoryRepository;
    private readonly ISuplierRepository _SupplierRepository;

    private const string CategoryKey = "cache_category";
    private const string CustomerKey = "cache_customer";
    private const string SupplierKey = "cache_supplier";
    private const string EmployeeKey = "cache_employee";

    private static readonly MemoryCacheEntryOptions CacheOptions = new()
    {
        //si no nadie accede a estos datos se borran en 30 minutos
        SlidingExpiration = TimeSpan.FromMinutes(30),
        //aunque se accedan los datos los datos expiraran en 2 horas
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
        Priority = CacheItemPriority.High
    };

    public IReadOnlyList<CategoryListDto> Categories =>
    _Cache.Get<List<CategoryListDto>>(CategoryKey)?.AsReadOnly()
    ?? new List<CategoryListDto>().AsReadOnly();

    public IReadOnlyList<CustomerListDto> Customers =>
    _Cache.Get<List<CustomerListDto>>(CustomerKey)?.AsReadOnly()
    ?? new List<CustomerListDto>().AsReadOnly();

    public IReadOnlyList<SupplierListDto> Suppliers =>
    _Cache.Get<List<SupplierListDto>>(SupplierKey)?.AsReadOnly()
    ?? new List<SupplierListDto>().AsReadOnly();

    public IReadOnlyList<EmployeeListDto> Employees =>
    _Cache.Get<List<EmployeeListDto>>(EmployeeKey)?.AsReadOnly()
    ?? new List<EmployeeListDto>().AsReadOnly();

    public AppCache
    (
        IMemoryCache cache,
        ICustomerRepository CustomerRepository,
        IEmployeeRepository EmployeeRepository,
        ISuplierRepository  SupplierRepository,
        ICategoryRepository categoryRepository
    )
    {
        _Cache = cache;
        _CustomerRepository = CustomerRepository;
        _CategoryRepository = categoryRepository;
        _EmployeeRepository = EmployeeRepository;
        _SupplierRepository = SupplierRepository;
    }

    public async Task LoadAsync()
    {
        var customers  =  _CustomerRepository.GetAllAsync();
        var categories =  _CategoryRepository.GetAllAsync();
        var employees =   _EmployeeRepository.GetAllAsync();
        var suppliers =   _SupplierRepository.GetAllAsync();

        //espera a que se ejecuten todas las acciones anteriores que son asincronas
        await Task.WhenAll(customers,categories,employees,suppliers);      

        _Cache.Set(CustomerKey,(await customers).ToListDtoList().ToList(),CacheOptions);
        _Cache.Set(CategoryKey,(await categories).ToCategoryListDto().ToList(),CacheOptions);
        _Cache.Set(EmployeeKey,(await employees).EmployeeTolistDto().ToList(),CacheOptions);
        _Cache.Set(SupplierKey,(await suppliers).SupplierToListDto().ToList(),CacheOptions);
    }

    public async Task ReloadCategoriesAsync()
    {
        var categories = await _CategoryRepository.GetAllAsync();
        _Cache.Set(CategoryKey, categories.ToCategoryListDto().ToList(),CacheOptions);
    }

    public async Task ReloadCustomerAsync()
    {
        var customers = await _CustomerRepository.GetAllAsync();
        _Cache.Set(CustomerKey, customers.ToListDtoList().ToList(),CacheOptions);
    }

    public async Task ReloadEmployeesAsync()
    {
        var employees = await _EmployeeRepository.GetAllAsync();
        _Cache.Set(EmployeeKey, employees.EmployeeTolistDto().ToList(),CacheOptions);
    }

    public async Task ReloadSuppliersAsync()
    {
        var suppliers = await _SupplierRepository.GetAllAsync();
        _Cache.Set(SupplierKey, suppliers.SupplierToListDto().ToList(),CacheOptions);
    }

}