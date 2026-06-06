using SistemaDistribuidora.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Interfaces; 

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    // Obtiene todos los empleados junto con las ventas que han realizado
    Task<IEnumerable<Employee>> GetAllEmployeesWithSaleAsync();

    // Obtiene todos los empleados que no han realizado ventas
    Task<IEnumerable<Employee>> GeAllEmployeedWithoutSaleAsync();

    // Obtiene todos los empleados junto con las compras que han realizado
    Task<IEnumerable<Employee>> GetAllWithPurchasesAsync();

    // Obtiene todos los empleados que no han realizado compras
    Task<IEnumerable<Employee>> GetByAllWithouPurchasesAsync();

}