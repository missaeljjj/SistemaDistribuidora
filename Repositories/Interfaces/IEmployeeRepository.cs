using SistemaDistribuidora.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Interfaces; 

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    // Obtiene todos los empleados junto con las ventas que han realizado
    Task<IEnumerable<(Employee employee, int QuantityOfSales)>> GetAllEmployeesWithQuantityofSaleAsync();

}