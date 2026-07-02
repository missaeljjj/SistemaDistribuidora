using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaDistribuidora.DTOs;

namespace SistemaDistribuidora.Services;
public interface IAppCache
{
    IReadOnlyList<CategoryListDto> Categories {get;}
    IReadOnlyList<CustomerListDto> Customers {get;}
    IReadOnlyList<SupplierListDto> Suppliers {get;}
    IReadOnlyList<EmployeeListDto> Employees {get;}
    Task LoadAsync(); 
    Task ReloadCategoriesAsync();
    Task ReloadEmployeesAsync();
    Task ReloadSuppliersAsync();
    Task ReloadCustomerAsync();
}