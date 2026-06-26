using SistemaDistribuidora.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Services.Interfaces;

public interface IEmployeeService
{
    Task CreateNewEmployee(CustomerCreateDto customercreatedto);

    Task UpdateEmployee(CustomerUpdateDto customerupdatedto);

    Task DeleteEmployee(int EmployeeId);

    Task<IEnumerable<EmployeeListDto>> GetAllEmployees(int EmployeeId);

    Task<IEnumerable<EmployeeDetailDto>> GetEmployeesDetail();
}
