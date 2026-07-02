using SistemaDistribuidora.Services.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Repositories.Interfaces;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Mappers;
using System.Linq;

namespace SistemaDistribuidora.Services.Implementation;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _EmployeeRepository;
    private readonly IAppCache _cache;

    public EmployeeService(IEmployeeRepository employeeRepository, IAppCache Cache)
    {
        _EmployeeRepository = employeeRepository;
        _cache = Cache;
    }

    public async Task CreateNewEmployee(EmployeeCreateDto dto)
    {

        var customer = dto.ToModel();

        await _EmployeeRepository.InsertAsync(customer);
        await _cache.ReloadEmployeesAsync();
    }

    public async Task UpdateEmployee(EmployeeUpdateDto dto)
    {
        var existing = await _EmployeeRepository.GetByIdAsync(dto.EmployeeId);

        bool duplicated = await _EmployeeRepository.EmployeeExistingForUpdate(dto.Identity ?? existing.IdentityCard, dto.EmployeeId);

        if (duplicated)
            throw new BussinessRulesException("Idenificacion duplicada", $"ya existe un empleado con identificacion: {dto.Identity}");

        var update = dto.ToModel(existing);

        await _EmployeeRepository.UpdateAsync(update);
        await _cache.ReloadEmployeesAsync();
    }

    public async Task DeleteEmployee(int EmployeeId)
    {
        await _EmployeeRepository.GetByIdAsync(EmployeeId);

        await _EmployeeRepository.DeleteAsync(EmployeeId);
        await _cache.ReloadEmployeesAsync();
    }

    public async Task<IEnumerable<EmployeeListDto>> GetAllEmployees()
    {
        if (_cache.Employees == null || !_cache.Employees.Any())
        {
            await _cache.ReloadEmployeesAsync();
        }

        return _cache.Employees ?? new List<EmployeeListDto>();
    }

    public async Task<IEnumerable<EmployeeDetailDto>> GetEmployeesDetail()
    {
        var EmployeeDetail = await _EmployeeRepository.GetAllEmployeesWithQuantityofSaleAsync();

        return EmployeeDetail.ToListDetail();
    }
}