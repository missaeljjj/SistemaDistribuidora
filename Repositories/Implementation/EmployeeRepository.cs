using SistemaDistribuidora.Models;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using SistemaDistribuidora.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace SistemaDistribuidora.Repositories.Implementation;
public class EmployeeRepository : IEmployeeRepository
{
    private readonly IDataBase _DataBase;

    public EmployeeRepository(IDataBase dataBase) 
    {
        _DataBase = dataBase;
    }

    public async Task InsertAsync(Employee employee) 
    { }

    public async Task DeleteAsync(int EmployeeId) 
    { }

    public async Task UpdateAsync(Employee employee)
    { }

    public async Task<Employee> GetByIdAsync(int EmployeeId) 
    {
        return null!;
    }
    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return null!;
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesWithQuantityofSaleAsync()
    {
        return null!;
    }

    
}

